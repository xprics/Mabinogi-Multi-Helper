# Mabinogi Multi Helper

마비노기 클라이언트를 CPU 분배로 다클라 성능 향상

- 이 프로그램은 다중 클라이언트 환경에서 마비노기의 CPU자원을 조절하여
- 좀 더 낮은 CPU사용량과 낮은 전력을 사용하도록 도와주는 프로그램입니다.

(Mabinogi CPU preferrence divider for lower usage of CPU and more less power consumption. )


refer : <https://cafe.naver.com/mabinogidsg/642329>

## Build & Test Environment

```Text
OS : Windows 10 Pro 64bit 20H2

Build : Visual Studio 2019

.Net Framework : 4.5
```
## TODO
  - 시스템 예약종료 or 클라이언트 예약 종료 작업이 잡혀있다면 프로그램 종료 할 때 정말 종료할지, 예약된 작업이 처리되지 않는다고 확인하게 하기


## Change Log


```Text
2021-06-15
    - 낮은 확률로 Thread UnSafe한 케이스 수정
    - 프로세스 예약 종료 시 목록 즉시 갱신되게 수정
    - 프로세스 예약 종료 시 종료하기 직전 마비노기 프로세스인지 한번 더 확인하게 수정
    - 시간 선택 화면 시간 포맷 '-'가 아닌 [년, 월 일]로 수정
    - 예약 종료 작업이 몇개나 되는지 기억하여 프로그램 종료 할 때 예약 작업 있다면 안내 문구 출력
    - 프로그램 실행 시 프로그램 버전 확인하여 최신과 다른 버전이면 안내문구 나오게 함
    - 닷넷 4.5로 다시 올림 ( HTTPS SSL 최신버전 Request를 위해 )
2021-06-14
    - 프로세스 숨김/보이기 기능
    - 프로세스 예약 종료 기능 추가
    - 시스템 예약 종료 기능 추가
    - 소스코드 최적화
    - BackgroundTaskManager 버그 수정
2021-06-11
    - add background task manager
2021-06-08
    - add trayicon
    - set activity window
    - memory GC fix
    - auto refresh process list
    - check same process is running
    - Re-design Program icon
    - add Program info dialog ( To show Program Version & Download Link, developer )
    - When application closing, ask to user whether close or put tray-icon.
    - Add ReleaseNote.txt for Normal Users.
    - Source Code relocate ( folder , namespace , ... )
    
    
    
##############이 아래 글자 버전 값 이외 수정 금지####################    
^^#$2021.06.15_REV_1.001
    
```
