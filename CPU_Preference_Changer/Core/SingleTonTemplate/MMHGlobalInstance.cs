using System;
using System.Threading;

namespace CPU_Preference_Changer.Core.SingleTonTemplate {

    /// <summary>
    /// by LT인척하는엘프. 2021.06.11 싱글톤 템플릿 구현
    /// 전역 인스턴스 인터페이스.
    /// 해당 클래스가 전역 객체임을 명시적으로 프로그래머에게 보여주기 
    /// + Release를 꼭 구현하게 하기 위함 (전역객체에서 해제해야할 데이터가 있다면 해당 로직 까먹지 말라고)
    /// </summary>
    public interface IMMHGlobalInstance {
        /// <summary>
        /// 프로그램 종료 시 전역객체 내용 초기화 함수
        /// </summary>
        void Release();
    }

    /// <summary>
    /// by LT인척하는엘프. 2021.06.11 싱글톤 템플릿 구현
    /// 특정객체를 로컬 객체, 프로그램 전역으로도 쓰고싶을거니까... 이렇게 구현하여 나중에
    /// 전역<->로컬 로직 변경 시 빠르게 대처 가능하게 하기 위함!
    /// 후발주자가 편리하게 싱글톤을 사용할 수 있게...
    /// class Test{
    ///      int a;
    /// }
    /// 라는 클래스가 있다면....
    /// 해당 클래스를 전역적으로 얻어야하는 어떤 함수 foo에서
    /// void foo(){
    ///         Test t = MMHGlobalInstance<Test>.GetInstance();
    /// }
    /// 하게되면 전역 객체를 로컬에 받아와서 사용하는 꼴이 된다...
    /// </summary>
    /// <typeparam name="T">템플릿 타입... 클래스타입을 사용할것.</typeparam>
    public abstract class MMHGlobalInstance<T> : IMMHGlobalInstance where T : class, new() {
        private static Lazy<T> mInstance = new Lazy<T>();
        private static Mutex mtx = new Mutex();
        protected MMHGlobalInstance() { }

        public T getInstance()
        {
            return GetInstance();
        }
        public void Release()
        {
            mInstance = null;
        }
        public static T GetInstance()
        {
            if (mInstance != null)
                return mInstance.Value;
            else {
                mtx.WaitOne();
                if (mInstance == null) {
                    mInstance = new Lazy<T>();
                }
                mtx.ReleaseMutex();

                return mInstance.Value;
            }
        }
    }
}
