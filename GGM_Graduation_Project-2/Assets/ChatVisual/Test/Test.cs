using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public class Test : MonoBehaviour
    {
        private UIDocument uiDocument;

        private VisualElement baseArea;
        private Label textAnim;

        public GameObject cube;

        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            var root = uiDocument.rootVisualElement;
            baseArea = root.Q<VisualElement>("BaseArea");
            textAnim = root.Q<Label>("TextAnim");

            ShakeChat();
        }

        public float duration = 1.0f; // 흔들기 지속 시간
        public float strength = 1; // 얼마나 멀리로 흔들리는지

        private void ShakeChat()
        {
            cube.transform.DOShakePosition(60f, .025f, 10, 40, false, false, ShakeRandomnessMode.Harmonic);

            Vector3 originalPosition = baseArea.transform.position;
            Vector3 randomOffset = Vector3.zero;
            float elapsed = 0f;

            DOTween.To(() => elapsed, x => elapsed = x, 1f, 0.25f)
                .OnStart(() =>
                {
                    float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
                    float x = strength * Mathf.Cos(randomAngle);
                    float y = strength * Mathf.Sin(randomAngle);

                    randomOffset = new Vector3(x, y, 0);
                    Debug.Log(randomOffset);
                })
                .OnUpdate(() =>
                {
                    Vector3 movePos = Vector3.zero;
                    if (elapsed < (duration / 2))       // 밖으로 나가는 중
                    {
                        movePos = Vector3.Lerp(originalPosition, randomOffset, elapsed / duration);
                    }
                    else
                    {
                        movePos = Vector3.Lerp(randomOffset, originalPosition, elapsed / duration);
                }
                    baseArea.transform.position = movePos;
                })
                .OnStepComplete(() =>
                {
                    baseArea.transform.position = originalPosition;

                    float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
                    float x = strength * Mathf.Cos(randomAngle);
                    float y = strength * Mathf.Sin(randomAngle);

                    randomOffset = new Vector3(x, y, 0);
                    Debug.Log(randomOffset);
                })
                .SetLoops(-1, LoopType.Restart);


            /*    DOTween.To(() => elapsed, x => elapsed = x, 1f, duration)
           .OnStart(() =>
           {
               float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
               float x = strength * Mathf.Cos(randomAngle);
               float y = strength * Mathf.Sin(randomAngle);

               randomOffset = new Vector3(x, y, 0);
               Debug.Log(randomOffset);
           })
           .OnUpdate(() =>
           {
               // 무작위 위치 오프셋을 생성
               Vector3 movePos = Vector3.Lerp(originalPosition, randomOffset, elapsed / duration);
               baseArea.transform.position = originalPosition + movePos;
               Debug.Log(1);
           })
           .OnComplete(() =>
           {
               // 애니메이션이 끝나면 원래 위치로 되돌립니다.
               baseArea.transform.position = originalPosition;
               Debug.Log(baseArea.transform.position);
           })
           .SetLoops(-1, LoopType.Restart);

            */

            textAnim.text = string.Empty;
            string m = "element.transform.position = new Vector3(100, 50, 0);";
            DOTween.To(() => textAnim.text, x => textAnim.text = x, m, 3f).SetEase(Ease.Linear);


        }
    }
}

/*
DOShakePosition의 작동원리

힘, 진동, 각도 이렇게 3가지가 있음.

힘은 말 그대로 최대로 움직이는 정도같고
진동은 빠르기 같음.
각도는 뭐 어디로 튕길건지 몇개까지 튕기는 각도가 생길건지 같음.

이걸 요요 써서 하니까..... 

 */