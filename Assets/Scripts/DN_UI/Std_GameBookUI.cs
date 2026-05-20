using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;


public class Std_GameBookUI : DaniTechUIBase
{
    [SerializeField] private GameObject prefab_Slot; //prefab_ 명시

    [Header("디테일 정보 영역")] //Inspetor 구분용
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private Text Text_Description;

    [SerializeField] private DaniTechUIButton Button_CloseUI;
    //[Header("부가 정보")] area하나로 묶어서 껐다켜기
    //[SerializeField] private GameObject Layout_Area;

    [Header("슬롯 리스트 영역")]
    [SerializeField] private Transform Transform_SlotRoot; // 스크롤뷰에 슬롯이 생성될 수 있게 위치를 지정하는 것

    private Dictionary<string, Std_GameBookSlotUI> _slotList = new Dictionary<string, Std_GameBookSlotUI>();

    private void OnEnable()
    {
        // 이 UI가 열릴때, 아이템 도감안에 있어야하는 데이터들을 불러온다.
        ReadItemListAndCreateSlot();

        Button_CloseUI.BindOnClickButtonEvent(Onclick_CloseGameBookUI);
    }

    public void Onclick_CloseGameBookUI()
    {
        DaniTechUIManager.Instance.CloseUI(DaniTechUIRootType.MainUI, DaniTechUIType.Std_GameBookUI);
    }

    private void OnDisable()
    {
        if(_slotList.Count > 0)
        {
            foreach (var slotKv in _slotList)
            {
                var slot = slotKv.Value; // 컴포넌트인데, 얘로 gameObject를 받아올 수 있다
                DestroyImmediate(slot.gameObject);
            }
            _slotList.Clear();
        }
    }


    private void ReadItemListAndCreateSlot()
    {
        Debug.Log("디버그테스트");

        // 데이터를 읽어와서 순회(foreach)하면서 아이템들을 도감 리스트에 표기
        var dataList = DaniTechGameDataManager.Instance.StdBazaarItemDataList;
        foreach (var dataKv in dataList)
        {
            var data = dataKv.Value;
            if (data == null) continue; //데이터가 Null일수 있으니 체크

            CreateGameBookSlot(data.Id);
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
    private void CreateGameBookSlot(string dataId)
    {
        var gObj = Instantiate(prefab_Slot, Transform_SlotRoot);
        if (gObj == null) return;
        
        // 게임 오브젝트는 동적 생성이 된 것
        var slotComponent = gObj.GetComponent<Std_GameBookSlotUI>();
        if (slotComponent == null) return;

        //동적 생서된 자식 슬롯(게임오브젝트)안에 있는 컴포넌트를 가져왔다.
        slotComponent.InitSlot(dataId, OnClickChildSlotSelected);
        _slotList.Add(dataId, slotComponent);

    }

    private void OnClickChildSlotSelected(string slotDataId)
    {
        var currentSelectedData = DaniTechGameDataManager.Instance.GetStdBazaarItem(slotDataId);
        if (currentSelectedData == null) return;
        Text_MainName.text = currentSelectedData.Name;
        Text_Description.text = currentSelectedData.Description;
        //Text_SellingPrice.text = currentSelectedData.SellingPrice;
        //Text_SellingPricce.gameObject.SetActive(currentSelectedData.SellingPrice > 0);
        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, currentSelectedData.IconPath).Forget(); // 아이콘이 비어있지 않다면 아이콘을 불러와서 세팅

        foreach (var slotKv in _slotList)
        {
            var slot = slotKv.Value;
            var dataId = slot.GetSlotDataId();
            slot.SetSelectedUI(slotDataId == dataId);
        }
    }
}
