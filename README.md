# MyPlayerSystem

언리얼 엔진의 Pawn 과 Controller 를 보고 만들어본 시스템.


Pawn 과 Controller 를 1:1로 매칭하여 값을 처리한다.

Controller 에 이동 방향, 회전 방향, 주시 방향을 Set 시키고,

Pawn은 Controller 여부에 따라 Controller 의 값 또는 default 값을 가지게 되는 구조.


Player의 Pawn 과 Controller 을 알기 위해 Scriptable 형태의 PlayerManager가 있음.


https://github.com/kimjisoo4/MyUtilities 가 필요함.

- 사용은 자유이나 그로 인해 생긴 오류에 대해서는 책임지지 않음.

자세한 정보 : --
