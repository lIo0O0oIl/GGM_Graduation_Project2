using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public class Test : MonoBehaviour
    {
        private UIDocument uiDocument;

        private VisualElement baseArea, baseArea2;
        private Label textAnim;
        private Button btn;

        public AudioClip hoverSound;
        private AudioSource audioSource;

        public GameObject cube, cube2;

        public bool is_ok = false;

        private void Update()
        {
            if (is_ok)
            {
                Debug.Log("is_OK!!!!");
            }
        }

        public void OnIs_Ok()
        {
            Debug.Log("click me");
            is_ok = true;
            is_ok = false;
        }

/*        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
        }

        private void Start()
        {
            var root = uiDocument.rootVisualElement;
            baseArea = root.Q<VisualElement>("BaseArea");
            baseArea2 = root.Q<VisualElement>("BaseArea2");
            textAnim = root.Q<Label>("TextAnim");
            btn = root.Q<Button>("Button1");

            audioSource = gameObject.GetComponent<AudioSource>();

            btn.RegisterCallback<MouseEnterEvent>(evt => {
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                audioSource.PlayOneShot(hoverSound);
                });

            ShakeChat();
        }*/

        public float duration; // 흔들기 지속 시간
        public float strength; // 얼마나 멀리로 흔들리는지

        //[Header("Cube2")]
        //public float dru

        private void ShakeChat()
        {
            cube.transform.DOShakePosition(60f, .025f, 10, 40, false, false, ShakeRandomnessMode.Harmonic);
            cube2.transform.DOShakePosition(.65f, .13f, 20, 50, false, false);


            #region 긴 진동
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
                    //Debug.Log(randomOffset);
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
                    //Debug.Log(randomOffset);
                })
                .SetLoops(-1, LoopType.Restart);
            #endregion


            Cube2();

            textAnim.text = string.Empty;
            string m = "element.transform.position = new Vector3(100, 50, 0);";
            DOTween.To(() => textAnim.text, x => textAnim.text = x, m, 3f).SetEase(Ease.Linear);


        }

        private void Cube2()
        {
            Vector3 originalPosition = baseArea2.transform.position;
            Vector3 randomOffset = Vector3.zero;
            float elapsed = 0f;
            float _duration = 1;
            float _strength = 40;

            DOTween.To(() => elapsed, x => elapsed = x, 1f, 0.03f)
                .OnStart(() =>
                {
                    float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
                    float x = _strength * Mathf.Cos(randomAngle);
                    float y = _strength * Mathf.Sin(randomAngle);

                    randomOffset = new Vector3(x, y, 0);
                    //Debug.Log(randomOffset);
                })
                .OnUpdate(() =>
                {
                    Vector3 movePos = Vector3.zero;
                    if (elapsed < (_duration / 2))       // 밖으로 나가는 중
                    {
                        movePos = Vector3.Lerp(originalPosition, randomOffset, elapsed / _duration);
                    }
                    else
                    {
                        movePos = Vector3.Lerp(randomOffset, originalPosition, elapsed / _duration);
                    }
                    baseArea2.transform.position = movePos;
                })
                .OnStepComplete(() =>
                {
                    baseArea2.transform.position = originalPosition;

                    float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
                    float x = _strength * Mathf.Cos(randomAngle);
                    float y = _strength * Mathf.Sin(randomAngle);

                    randomOffset = new Vector3(x, y, 0);
                    //Debug.Log(randomOffset);
                })
                .SetLoops(20, LoopType.Restart);
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