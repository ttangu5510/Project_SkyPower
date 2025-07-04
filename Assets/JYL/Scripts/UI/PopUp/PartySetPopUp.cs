using KYG_skyPower;
using LJ2;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JYL
{
    public class PartySetPopUp : BaseUI
    {
        private string iconPrefabPath = "JYL/UI/CharacterIconPrefab";
        private CharacterSaveLoader characterLoader;
        private Image mainIllustImg;
        private Image sub1IllustImg;
        private Image sub2IllustImg;
        private GameObject iconPrefab;
        private RectTransform parent;
        private List<GameObject> iconList;
        private Dictionary<string, CharactorController> charDict;
        private List<CharacterSave> charDataList;
        public static bool isPartySetting = false;
        private bool isMainSet = false;
        private bool isSub1Set = false;
        private bool isSub2Set = false;

        private TMP_Text warningText;
        private new void Awake()
        {
            base.Awake();
            Init();
        }
        private void OnEnable() { }
        void Start()
        {
             // TODO : 상점 연결
            characterLoader = GetComponent<CharacterSaveLoader>();
            GetEvent("PSCharImg1").Click += OpenInvenPopUp;
            //GetEvent("PSCharImg2").Click += OpenInvenPopUp;
            //GetEvent("PSCharImg3").Click += OpenInvenPopUp;
            CreateIcons();
            warningText.gameObject.SetActive(false);
        }

        void Update()
        {
            // 파티 세팅 중이면
            if (isPartySetting)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    characterLoader.GetCharPrefab();
                    CreateIcons();
                    Util.ConsumeESC();
                    isPartySetting = false;
                }
                warningText.gameObject.SetActive(true);
            }
            // 모든 파티 세팅 완료
            if (isMainSet && isSub1Set && isSub2Set)
            {
                warningText.gameObject.SetActive(false);
            }
            else
            {
                isPartySetting = true;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Util.ConsumeESC();
                }
                warningText.gameObject.SetActive(true);
            }
        }

        // 초기화
        private void Init()
        {
            charDict = new Dictionary<string, CharactorController>();
            charDataList = Manager.Game.CurrentSave.characterInventory.characters;
            iconList = new List<GameObject>();
            characterLoader = GetComponent<CharacterSaveLoader>();
            //canvasGroup = GetComponent<CanvasGroup>();
            mainIllustImg = GetUI<Image>("PSCharImg1");
            sub1IllustImg = GetUI<Image>("PSCharImg2");
            sub2IllustImg = GetUI<Image>("PSCharImg3");
            parent = GetUI<RectTransform>("Content");
            warningText = GetUI<TMP_Text>("PartySetWarningText");
            //popUpPanel = GetUI<RectTransform>("PartySetPopUp");
            iconPrefab = Resources.Load<GameObject>(iconPrefabPath);
            characterLoader.GetCharPrefab();
        }

        // 장비창 오픈
        private void OpenInvenPopUp(PointerEventData eventData)
        {
            // 선택된 캐릭을 기준으로 인벤을 연다
            Util.ExtractTrailNumber(eventData.pointerClick.name, out int index);
            // GameManager.Instance.selectSave.party[index] -> 캐릭터 ID
            // 캐릭터 컨트롤러 (캐릭터 ID)
            UIManager.Instance.selectIndexUI = index;
            if(index == 1)
            {
                int mainCharIndex = charDataList.FindIndex(c => c.partySet == PartySet.Main);
                if (mainCharIndex == -1)
                {
                    Debug.Log($"메인 캐릭터는 현재 비어있음");
                }
                else if(!isPartySetting)
                {
                    UIManager.Instance.ShowPopUp<InvenPopUp>();
                }
            }
        }

        // 아이콘 및 일러스트들을 생성
        private void CreateIcons()
        {
            if (iconList.Count > 0)
            {
                foreach (GameObject icon in iconList)
                {
                    GameObject outIcon = DeleteFromDictionary(icon.gameObject.name, icon.gameObject);
                    Destroy(outIcon);
                }
                iconList.Clear();
                charDict.Clear();
            }

            int imgIndex = 0;
            isMainSet = false;
            isSub1Set = false;
            isSub2Set = false;
            foreach (CharactorController character in characterLoader.charactorController)
            {
                // 레벨 1 이상인 경우에만 이벤트 등록. 소유중인 캐릭터들임
                if (character.level > 0)
                {
                    GameObject go;
                    go = Instantiate(iconPrefab, parent);
                    go.name = $"StayCharImg{imgIndex + 1}";
                    // TODO Add Test
                    AddUIToDictionary(go.gameObject);
                    imgIndex++;
                    go.GetComponentInChildren<Image>().sprite = character.image;
                    GetEvent($"{go.name}").Drag += BeginIconDrag;
                    GetEvent($"{go.name}").Drag += IconDrag;
                    GetEvent($"{go.name}").EndDrag += OnIconDragEnd;
                    iconList.Add(go);
                    if (!charDict.TryAdd(go.name, character))
                    {
                        Debug.LogWarning($"이미 charDict에 있음{go.name}");
                    }
                }
                //TODO: 에셋으로 아이콘과 일러스트가 들어오면 넣어줘야 함

                switch (character.partySet)
                {
                    case PartySet.Main:
                        mainIllustImg.sprite = character.image;
                        isMainSet = true;
                        break;
                    case PartySet.Sub1:
                        sub1IllustImg.sprite = character.image;
                        isSub1Set = true;
                        break;
                    case PartySet.Sub2:
                        sub2IllustImg.sprite = character.image;
                        isSub2Set = true;
                        break;
                }
            }
            CheckPartySlotNull();
 
        }
        private void CheckPartySlotNull()
        {
            // 메인
            if (!isMainSet)
            {
                mainIllustImg.sprite = null;
                Color c = mainIllustImg.color;
                c.a = 0f;
                mainIllustImg.color = c;
            }
            else
            {
                Color c = mainIllustImg.color;
                c.a = 1f;
                mainIllustImg.color = c;
            }
            // 서브1
            if (!isSub1Set)
            {
                sub1IllustImg.sprite = null;
                Color c = sub1IllustImg.color;
                c.a = 0f;
                sub1IllustImg.color = c;
            }
            else
            {
                Color c = sub1IllustImg.color;
                c.a = 1f;
                sub1IllustImg.color = c;
            }
            // 서브2
            if (!isSub2Set)
            {
                sub2IllustImg.sprite = null;
                Color c = sub2IllustImg.color;
                c.a = 0f;
                sub2IllustImg.color = c;
            }
            else
            {
                Color c = sub2IllustImg.color;
                c.a = 1f;
                sub2IllustImg.color = c;
            }
        }
        // 아이콘 드래그 시작
        private void BeginIconDrag(PointerEventData eventData)
        {
            GameObject dragIcon = GetUI($"{eventData.pointerDrag.gameObject.name}");
            dragIcon.transform.SetParent(transform.root);
            isPartySetting = true;
        }

        // 드래그 중
        private void IconDrag(PointerEventData eventData)
        {
            GetUI($"{eventData.pointerDrag.gameObject.name}").transform.position = eventData.position;
        }

        //드래그 끝
        private void OnIconDragEnd(PointerEventData eventData)
        {
            GameObject selectedIcon = GetUI($"{eventData.pointerDrag.gameObject.name}");
            if (selectedIcon == null) return;

            //드롭 위치의 UI들을 검색함
            List<RaycastResult> results = new List<RaycastResult>();
            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = eventData.position;
            GraphicRaycaster raycaster = GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();
            raycaster.Raycast(ped, results); //ped 위치(드래그끝난위치)로 레이를 발사해서 리스트로 결과를 담음

            // 현재 아이콘의 값을 기준으로 캐릭터를 찾음
            charDict.TryGetValue($"{selectedIcon.name}", out CharactorController character); // 여기서 드래그중인 아이콘의 캐릭터 컨트롤러 나옴
            if (character == null)
            {
                Debug.LogWarning($"드래그 중인 아이콘의 캐릭터컨트롤러가 딕셔너리에 없음{selectedIcon.name}");
            }
            // 드래그하던 캐릭터의 정보
            CharacterSave dragCharData = charDataList.Find(c => c.id == character.id);
            int dragCharDataIndex = charDataList.FindIndex(c => c.id == character.id);

            // 모든 레이캐스트 결과를 비교
            foreach (RaycastResult result in results)
            {
                GameObject targetSlot = result.gameObject;
                // 만약, 해당 타겟UI의 태그가 "파티슬롯"이면 우리가 찾던 UI다.
                if (targetSlot.CompareTag("PartySlot"))
                {
                    // UI 오브젝트의 이름을 통해 메인, 서브를 판별
                    Util.ExtractTrailNumber($"{targetSlot.name}", out int slotNum);
                    // 만약, 드래그 중인것과 내려놓는곳이 같다면 작업을 하지 않는다
                    if ((int)dragCharData.partySet+1 != slotNum)
                    {
                        switch (slotNum)
                        {
                            case 1: //메인
                                    // 메인캐릭터 정보 가져오기
                                CharacterSave mainCharData = charDataList.Find(c => c.partySet == PartySet.Main);
                                int mainCharIndex = charDataList.FindIndex(c => c.partySet == PartySet.Main);
                                if (mainCharIndex == -1)
                                {
                                    Debug.Log($"메인 캐릭터는 현재 비어있음");
                                }
                                else
                                {
                                    mainCharData.partySet = PartySet.None;
                                    charDataList[mainCharIndex] = mainCharData;
                                }

                                // 드래그 하던 애를 메인으로 올림
                                dragCharData.partySet = PartySet.Main;
                                charDataList[dragCharDataIndex] = dragCharData;
                                break;
                            case 2: //서브1
                                CharacterSave sub1CharData = charDataList.Find(c => c.partySet == PartySet.Sub1);
                                int sub1CharIndex = charDataList.FindIndex(c => c.partySet == PartySet.Sub1);
                                if (sub1CharIndex == -1)
                                {
                                    Debug.Log($"서브1 캐릭터는 현재 비어있음");
                                }
                                else
                                {
                                    sub1CharData.partySet = PartySet.None;
                                    charDataList[sub1CharIndex] = sub1CharData;
                                }

                                dragCharData.partySet = PartySet.Sub1;
                                charDataList[dragCharDataIndex] = dragCharData;
                                break;
                            case 3: //서브2
                                CharacterSave sub2CharData = charDataList.Find(c => c.partySet == PartySet.Sub2);
                                int sub2CharIndex = charDataList.FindIndex(c => c.partySet == PartySet.Sub2);
                                if (sub2CharIndex == -1)
                                {
                                    Debug.Log($"서브2 캐릭터는 현재 비어있음");
                                }
                                else
                                {
                                    sub2CharData.partySet = PartySet.None;
                                    charDataList[sub2CharIndex] = sub2CharData;
                                }

                                dragCharData.partySet = PartySet.Sub2;
                                charDataList[dragCharDataIndex] = dragCharData;
                                break;
                        }
                    }
                }
            }

            // 함수 종료 시점에 다시 초기화 함
            characterLoader.GetCharPrefab();
            CreateIcons();
            isPartySetting = false;
        }
    }
}