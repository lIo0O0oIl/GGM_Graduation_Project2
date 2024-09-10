# GGM_Graduation_Project2
GGM 2024-2 졸업 작품 프로젝트 입니다.

<br>

<details><summary>채팅 관리 커스텀 에디터 구조도</summary>

<br>
커스텀 에디터를 활용해 제작한 채팅 에디터에 대한 다이어그램입니다.

(각 이름을 누르면 해당 코드로 이동할 수 있습니다.)

![image](https://github.com/user-attachments/assets/17da37a2-8dd8-45c0-9528-2d022dcc2737)


1. UI Toolkit를 사용해 CustomElement를 제작하여 창을 제작하였습니다.
- [HierarchyView](https://github.com/DDOBAGI-GGM/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Editor/CustomElement/HierarchyView.cs)
- [InspectorView](https://github.com/DDOBAGI-GGM/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Editor/CustomElement/InspectorView.cs)
- [ChatView](https://github.com/DDOBAGI-GGM/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Editor/CustomElement/ChatView.cs)

<br>

2. 에디터 내에서 사용하는 스크립트들 입니다.
- [ChatEditor](https://github.com/DDOBAGI-GGM/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Editor/ChatEditor.cs) (창 생성 및 관리)
- [ChatContainer](https://github.com/DDOBAGI-GGM/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/ChatContainer.cs) (에디터 내부 데이터 처리)
- [NodeView](https://github.com/DDOBAGI-GGM/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Editor/NodeView/NodeView.cs) (만들어지는 노드 관리)

<br>

3. 채팅의 진행을 위해 ScriptableObject인 Node를 상속받는 클래스들을 사용하여 여러 채팅 타입의 데이터를 저장합니다.
- [Node](https://github.com/DDOBAGI-GGM/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Node/Node.cs)
- [RootNode](https://github.com/DDOBAGI-GGM/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Node/RootNode.cs)
- [ChatNode](https://github.com/DDOBAGI-GGM/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Node/ChatNode.cs)
- [AskNode](https://github.com/DDOBAGI-GGM/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Node/AskNode.cs)
- [ConditionNode﻿](https://github.com/DDOBAGI-GGM/GGM_Graduation_Project2/blob/main/GGM_Graduation_Project-2/Assets/ChatVisual/Node/ConditionNode.cs)

</details>
