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

    private event Action<string, EGameBookCategoryType> _onclickSlot;
    // private DNItemData _data; // 데이터를 안 가지고 있는이유 = 다양한 데이터들을 도감에서 보여줄 수 있기 때문에 슬롯이 특정 데이터 타입에 종속적이지 않게 하기 위해서, 슬롯이 자기가 어떤 데이터인지 식별할 수 있는 DataId만 보관하는 형태로 구현한다.

    private string _slotDataId; // 슬롯이 자기가 살아있는 동안 어떤 슬롯인지 DataId를 보관하는 변수
    private EGameBookCategoryType _categoryType;

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
        _onclickSlot?.Invoke(_slotDataId, _categoryType);
    }

    private void OnDisable()
    {
        _onclickSlot = null;
    }

    private void SetSlotUI(string dataName, string iconPath)
    {
        Text_MainName.text = dataName; // 이름 반영
        if (string.IsNullOrEmpty(iconPath) == false) // 아이콘이 있다면
        {
            // 이건 잘 만들어 둔거니까 묻지마 사용...< Image에 아이콘, Spitre리소스 불러와서 표기해줄때
            DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, iconPath).Forget();
        }
    }
    public void InitSlot(string dataId, EGameBookCategoryType curCategoryType, Action<string, EGameBookCategoryType> onClickCallBack /*TableType*/) // TODO: 나중에 카테고리에 따라 다른 데이터를 받아올 수 있도록 파라미터를 추가해서 구별할 필요가 있다.
    {
        if (curCategoryType == EGameBookCategoryType.StdBazaarItem)
        {
            var StdBazaarData = DaniTechGameDataManager.Instance.GetStdBazaarItem(dataId);
            if (StdBazaarData == null) return;

            SetSlotUI(StdBazaarData.Name, StdBazaarData.IconPath);
        }
        else if (curCategoryType == EGameBookCategoryType.DNItem)
        {
            var DNItemData = DaniTechGameDataManager.Instance.GetDNItemData(dataId);
            if (DNItemData == null) return;

            SetSlotUI(DNItemData.Name, DNItemData.IconPath);
        }
        else if (curCategoryType == EGameBookCategoryType.DNMonster)
        {
            var DNMonsterData = DaniTechGameDataManager.Instance.GetDNMonsterData(dataId);
            if (DNMonsterData == null) return;

            SetSlotUI(DNMonsterData.Name, DNMonsterData.IconPath);
        }

        // 데이터를 잘 받아 왔으면
        _slotDataId = dataId;
        _categoryType = curCategoryType;
        _onclickSlot += onClickCallBack; // 이벤트 등록 완료

    }

    public void SetSelectedUI(bool isSelected)
    {
        Gobj_Selected.SetActive(isSelected);
    }
}
