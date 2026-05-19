using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using JetBrains.Annotations;

public class Std_GameBookSlotUI : MonoBehaviour
{
    [Header("디테일 정보 영역")] //Inspetor 구분용
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private GameObject Gobj_Selected; // 활성비활성화 기능으로만 이용할거라서 (용도가 이미지가 아니라서)
    [SerializeField] private DaniTechUIButton Button_SlotClick;

    private event Action<string> _onclickSlot;
    // private DNItemData _data; // 데이터를 안 가지고 있는이유 = 다양한 데이터들을 도감에서 보여줄 수 있기 때문에 슬롯이 특정 데이터 타입에 종속적이지 않게 하기 위해서, 슬롯이 자기가 어떤 데이터인지 식별할 수 있는 DataId만 보관하는 형태로 구현한다.

    private string _slotDataId; // 슬롯이 자기가 살아있는 동안 어떤 슬롯인지 DataId를 보관하는 변수

    public string GetSlotDataId()
    {
        return _slotDataId;
    }

    private void OnEnable()
    {
        // 그냥 버튼 눌릴게 할려고?
        Button_SlotClick.BindOnClickButtonEvent(OnClick_GameBookSlot);
    }

    public void OnClick_GameBookSlot()
    {
        // 자식이 눌려진걸 부모에게 알림
        _onclickSlot?.Invoke(_slotDataId);
    }

    private void OnDisable()
    {
        _onclickSlot = null;
    }

    public void InitSlot(string dataId, Action<string> onClickCallBack /*TableType*/) // TODO: 나중에 카테고리에 따라 다른 데이터를 받아올 수 있도록 파라미터를 추가해서 구별할 필요가 있다.
    {
        var itemData = DaniTechGameDataManager.Instance.GetDNItemData(dataId);
        if (itemData == null) return;

        Text_MainName.text = itemData.Name;

        string iconPath = itemData.IconPath;
        if (string.IsNullOrEmpty(itemData.IconPath) == true) return; // 혹시 누군가 비울 수 있으니 Null체크
        
        // 이미지에 아이콘, 스프라이트 소스 불러와서 표기
        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, iconPath).Forget(); // 아이콘이 비어있지 않다면 아이콘을 불러와서 세팅

        // 데이터를 잘 받아 왔으면
        _slotDataId = dataId;

        _onclickSlot += onClickCallBack; // 이벤트 등록 완료

    }

    public void SetSelectedUI(bool isSelected)
    {
        Gobj_Selected.SetActive(isSelected);
    }
}
