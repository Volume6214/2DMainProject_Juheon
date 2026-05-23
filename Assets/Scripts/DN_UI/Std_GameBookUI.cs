using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public enum EGameBookCategoryType
{
    None = 0,
    StdBazaarItem,
    DNItem,
    DNMonster
}
public class Std_GameBookUI : DaniTechUIBase
{
    [Header("동적 생성할 프리팹")]
    [SerializeField] private GameObject prefab_Slot; //게임 오브젝트이지만, 프리팹이라는 단어를 명시

    [Header("디테일 정보 영역")] //Inspetor 구분용
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private Text Text_Description;

    [Header("슬롯 리스트 영역")]
    [SerializeField] private Transform Transform_SlotRoot; // 스크롤뷰에 슬롯이 생성될 수 있게 위치를 지정하는 것

    [Header("카테고리영역")]
    [SerializeField] private DaniTechUIButton Button_StdBazaarItemCategory;
    [SerializeField] private DaniTechUIButton Button_DNItemCategory;
    [SerializeField] private DaniTechUIButton Button_DNMonsterCategory;

    [SerializeField] private DaniTechUIButton Button_CloseUI;
    //[Header("부가 정보")] area하나로 묶어서 껐다켜기
    //[SerializeField] private GameObject Layout_Area;

    private Dictionary<string, Std_GameBookSlotUI> _slotList = new Dictionary<string, Std_GameBookSlotUI>();

    private void OnEnable()
    {
        // 이 UI가 열릴때, 아이템 도감안에 있어야하는 데이터들을 불러온다.
        Onclick_StdBazaarItem();

        Button_CloseUI.BindOnClickButtonEvent(Onclick_CloseGameBookUI);
        Button_StdBazaarItemCategory.BindOnClickButtonEvent(Onclick_StdBazaarItem);
        Button_DNItemCategory.BindOnClickButtonEvent(Onclick_DNItem);
        Button_DNMonsterCategory.BindOnClickButtonEvent(Onclick_DNMonster);
    }

    private void OnDisable()
    {
        DestroyAndClearSlotList();
    }

    private void DestroyAndClearSlotList()
    {
        if (_slotList.Count > 0)
        {
            foreach (var slotKv in _slotList)
            {
                var slot = slotKv.Value; // 컴포넌트인데, 얘로 gameObject를 받아올 수 있다
                DestroyImmediate(slot.gameObject);
            }
            _slotList.Clear();
        }
    }

    public void Onclick_CloseGameBookUI()
    {
        DaniTechUIManager.Instance.CloseUI(DaniTechUIRootType.MainUI, DaniTechUIType.Std_GameBookUI);
    }



    public void Onclick_StdBazaarItem()
    {
        SetLayoutGameBookByCategory(EGameBookCategoryType.StdBazaarItem);
    }

    public void Onclick_DNItem()
    {
        SetLayoutGameBookByCategory(EGameBookCategoryType.DNItem);
    }

    public void Onclick_DNMonster()
    {
        SetLayoutGameBookByCategory(EGameBookCategoryType.DNMonster);
    }


    public void SetLayoutGameBookByCategory(EGameBookCategoryType categoryType)
    {
        DestroyAndClearSlotList();

        switch (categoryType)
        {
            case EGameBookCategoryType.StdBazaarItem:
                ReadStdBazaarItemListAndCreateSlot();
                break;
            case EGameBookCategoryType.DNItem:
                ReadDNItemListAndCreateSlot();
                break;
            case EGameBookCategoryType.DNMonster:
                ReadDNMonsterListAndCreateSlot();
                break;
        }
            
    }

    private void ReadStdBazaarItemListAndCreateSlot()
    {
        // 데이터를 읽어와서 순회(foreach)하면서 아이템들을 도감 리스트에 표기
        var dataList = DaniTechGameDataManager.Instance.StdBazaarItemDataList;
        foreach (var dataKv in dataList)
        {
            var data = dataKv.Value;
            if (data == null) continue; //데이터가 Null일수 있으니 체크

            CreateGameBookSlot(data.Id, EGameBookCategoryType.StdBazaarItem);
        }

        if (_slotList.Count > 0)
        {
            foreach (var slotKv in _slotList)
            {
                var slot = slotKv.Value;
                slot.OnClick_GameBookSlot();
            }
        }
    }

    private void ReadDNItemListAndCreateSlot()
    {
        var dataList = DaniTechGameDataManager.Instance.ItemDataList;
        foreach (var dataKv in dataList)
        {
            var data = dataKv.Value;
            if (data == null) continue; 

            CreateGameBookSlot(data.Id, EGameBookCategoryType.DNItem);
        }

        if(_slotList.Count > 0)
        {
            foreach(var slotKv in _slotList)
            {
                var slot = slotKv.Value;
                slot.OnClick_GameBookSlot();
            }
        }
    }

    private void ReadDNMonsterListAndCreateSlot()
    {
        var dataList = DaniTechGameDataManager.Instance.MonsterDataList;
        foreach (var dataKv in dataList)
        {
            var data = dataKv.Value;
            if (data == null) continue;

            CreateGameBookSlot(data.Id, EGameBookCategoryType.DNMonster);
        }

        if(_slotList.Count > 0)
        {
            foreach(var slotKv in _slotList)
            {
                var slot = slotKv.Value;
                slot.OnClick_GameBookSlot();
            }
        }
    }


    // 슬롯 1개 제대로 생서해주는 메서드
    private void CreateGameBookSlot(string dataId, EGameBookCategoryType curCategoryType)
    {
        var gObj = Instantiate(prefab_Slot, Transform_SlotRoot);
        if (gObj == null) return;
        
        // 게임 오브젝트는 동적 생성이 된 것
        var slotComponent = gObj.GetComponent<Std_GameBookSlotUI>();
        if (slotComponent == null) return;

        //동적 생서된 자식 슬롯(게임오브젝트)안에 있는 컴포넌트를 가져왔다.
        slotComponent.InitSlot(dataId, curCategoryType ,OnClickChildSlotSelected);
        _slotList.Add(dataId, slotComponent);

    }

    private void SetDetailInfoUI(string dataName, string dataDescription = "", string iconPath = "")
    {
        // Text_SellingPrice.text = currentSelectedData.SellingPrice;
        // Text_SellingPrice.gameObject.SetActive(currentSelectedData.SellingPrice > 0);
        Text_MainName.text = dataName;
        Text_Description.text = dataDescription;
        if (string.IsNullOrEmpty(iconPath) == false)
        {
            DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, iconPath).Forget();
        }
    }

    private void OnClickChildSlotSelected(string slotDataId, EGameBookCategoryType categoryType)
    {
        if (categoryType == EGameBookCategoryType.StdBazaarItem)
        {
            var currentSelectedData = DaniTechGameDataManager.Instance.GetStdBazaarItem(slotDataId);
            if (currentSelectedData == null) return;
            SetDetailInfoUI(currentSelectedData.Name, currentSelectedData.Description, currentSelectedData.IconPath);

            foreach (var slotKv in _slotList)
            {
                var slot = slotKv.Value;
                var dataId = slot.GetSlotDataId();
                slot.SetSelectedUI(slotDataId == dataId);
            }
        }
        else if (categoryType == EGameBookCategoryType.DNItem)
        {
            var currentSelectedData = DaniTechGameDataManager.Instance.GetDNItemData(slotDataId);
            if (currentSelectedData == null) return;
            SetDetailInfoUI(currentSelectedData.Name, currentSelectedData.Description, currentSelectedData.IconPath);
        }
        else if (categoryType == EGameBookCategoryType.DNMonster)
        {
            var currentSelectedData = DaniTechGameDataManager.Instance.GetDNMonsterData(slotDataId);
            if (currentSelectedData == null) return;
            SetDetailInfoUI(currentSelectedData.Name, currentSelectedData.Description, currentSelectedData.IconPath);
        }

        foreach (var slotKv in _slotList)
        {
            var slot = slotKv.Value;
            var dataId = slot.GetSlotDataId();
            slot.SetSelectedUI(slotDataId == dataId);
        }
    }
}
