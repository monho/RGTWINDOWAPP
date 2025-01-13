# RGT Order Management System

## 📋 프로젝트 개요
**RGT Order Management System**은 패널 UI와 데이터 그리드뷰(DataGridView)를 사용하여 실시간 주문 데이터를 관리하고 통계 대시보드를 제공하는 데스크톱 애플리케이션입니다.  
이 애플리케이션은 주문 데이터 생성을 시뮬레이션하거나 백그라운드 서비스로 실행하여 실시간 데이터 관리를 지원합니다.

---

## 🛠 주요 기능

### 1. 실시간 주문 관리
- 주문 데이터를 패널 형태로 시각적으로 표시.
- 각 테이블 패널에 주문 상태(주문 접수, 주문 처리, 처리 완료)를 실시간으로 반영.
- 신규 주문 시 테이블 패널이 깜빡이는 시각적 효과 제공.

### 2. 통계 대시보드
- **DataGridView**를 활용해 일별/시간대별 주문 데이터를 표시.
- 실시간으로 통계 데이터를 업데이트.
- 날짜와 시간대별 주문량을 구체적으로 보여줌.

### 3. 주문 데이터 시뮬레이션
- `OrderSimulator`를 통해 임의의 주문 데이터를 생성.
- 이벤트 기반 구조로 주문 데이터를 실시간으로 처리.

### 4. 백그라운드 실행
- 윈도우 서비스로 `OrderService`를 구현하여 백그라운드에서 주문 데이터 처리 가능.

---

## 💻 기술 스택

### 언어 및 프레임워크
- **C# (.NET Framework)**
- **Windows Forms (WinForms)**

### 주요 라이브러리
- **System.Timers** - 주문 데이터 시뮬레이션을 위한 타이머 구현
- **Newtonsoft.Json** - JSON 직렬화 및 역직렬화

---

## 🗂 프로젝트 구조
```plaintext
RGT/
├── RGT.Core/
│   ├── Entities/
│   │   └── OrderData.cs         # 주문 데이터를 정의하는 엔티티
│   └── Services/
│       └── IStatisticsManager.cs # 통계 관리 인터페이스
├── RGT.Infrastructure/
│   ├── Services/
│   │   ├── OrderSimulator.cs    # 주문 데이터를 시뮬레이션하는 서비스
│   │   └── OrderService.cs      # 윈도우 서비스 구현
├── RGT/
│   ├── Form1.cs                 # 메인 UI 폼
│   ├── Program.cs               # 애플리케이션 진입점
│   └── App.config               # 윈도우 서비스 구성
└── README.md                    # 프로젝트 설명서
