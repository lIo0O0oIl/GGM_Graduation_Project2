# GGM_Graduation_Project2
GGM 2024-2 졸업 작품 프로젝트 입니다.

<br>

<details><summary>채팅 관리 커스텀 에디터 구조도</summary>

<br>
제작한 채팅 관리 커스텀 에디터 입니다. 각 인물마다 채팅을 저장하고 추가하며 관리할 수 있습니다.

노드를 추가하여 인물의 표정, 대사, 대사 타입, 질문, 대화 조건 등을 정의할 수 있습니다.

![CustomEditor](https://github.com/user-attachments/assets/77aff180-943b-4a62-acaf-e6a10b05daac)

<br>
커스텀 에디터를 활용해 제작한 채팅 에디터에 대한 다이어그램입니다.

![image](https://github.com/user-attachments/assets/17da37a2-8dd8-45c0-9528-2d022dcc2737)

(각 이름을 누르면 해당 코드로 이동할 수 있습니다.)

1. UI Toolkit를 사용해 CustomElement를 제작하여 창을 제작하였습니다.

- [HierarchyView](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Editor/CustomElement/HierarchyView.cs)
- [InspectorView](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Editor/CustomElement/InspectorView.cs)
- [ChatView](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Editor/CustomElement/ChatView.cs)

<br>

2. 에디터 내에서 사용하는 스크립트들 입니다.
- [ChatEditor](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Editor/ChatEditor.cs) (창 생성 및 관리)
- [ChatContainer](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/ChatContainer.cs) (에디터 내부 데이터 처리)
- [NodeView](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Editor/NodeView/NodeView.cs) (만들어지는 노드 관리)

<br>

3. 채팅의 진행을 위해 ScriptableObject인 Node를 상속받는 클래스들을 사용하여 여러 채팅 타입의 데이터를 저장합니다.
- [Node](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Node/Node.cs)
- [RootNode](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Node/RootNode.cs)
- [ChatNode](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Node/ChatNode.cs)
- [AskNode](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Node/AskNode.cs)
- [ConditionNode﻿](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Node/ConditionNode.cs)

</details>

<br>

<details><summary>UI Toolkit 관련 코드</summary>

<br>
UI ToolKit의 Element에 Manipulator를 추가하는 방법으로 드래그 기능, 스크롤 및 줌 기능을 제작하였습니다.

![Dragger](https://github.com/user-attachments/assets/54f01e69-7e7a-42a8-9593-ca005b130450)

![Move](https://github.com/user-attachments/assets/c8396cf0-4947-443f-a83c-7608306b03c1)

(각 이름을 누르면 해당 코드로 이동할 수 있습니다.)

- [Dragger](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/01.Scripts/SE/CustemElement/Dragger.cs) (드래그 기능)
- [DraggerScrollView](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/01.Scripts/SE/CustemElement/DraggerScrollView.cs) (스크롤 및 줌 기능)

</details>

<br>

<details><summary>채팅 진행 로직, 창 변경 로직, 인물관계도 관련 코드</summary>

<br>
(각 이름을 누르면 해당 코드로 이동할 수 있습니다.)

SO에서 읽어온 채팅 데이터를 통해서 채팅을 진행합니다.

![Chat](https://github.com/user-attachments/assets/67de6aa8-f31c-4c40-bc32-650cd32558ad)

- [ChatHumanManager](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/01.Scripts/tjfdk/Manager/ChatHumanManager.cs)

<br>

창이 바뀌는 로직을 간단히 제작하였습니다. (해당 코드에는 창 기능 뿐만아니라 여러 UI 관련 기능이 담겨있습니다.)

![0810](https://github.com/user-attachments/assets/0d22e9d9-6274-4c07-8e8d-5051434c5563)

- [UIReader_Main](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/01.Scripts/tjfdk/UIReader/UIReader_Main.cs)

<br>

제작중인 인물관계도에 대한 코드를 담은 폴더입니다. 인물관계도의 배경, 그림 이동, 선 연결 등의 기능이 제작되어있습니다.

![0906_2](https://github.com/user-attachments/assets/060a7fc9-c88d-4f11-892c-d97e48282c38)

- [RelationshipSystem](https://github.com/lIo0O0oIl/GGM_Graduation_Project2/tree/main/GGM_Graduation_Project-2/Assets/01.Scripts/SE/RelationshipSystem)

</details>
