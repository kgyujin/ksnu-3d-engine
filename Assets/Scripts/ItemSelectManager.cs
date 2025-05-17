using UnityEngine;
public class ItemSelectManager : MonoBehaviour
{
    public GameObject[] Item;            // 아이템 프리팹 배열
    public Transform weaponslot;         // 손 본에 붙은 WeaponSlot (예: RightHand > WeaponSlot)
    private GameObject currentEquippedItem;
    private RaycastObjectMover raycastObjectMover;

    // 숫자키에 등록된 지팡이를 저장하는 배열
    private GameObject[] registeredWands = new GameObject[3];

    void Start()
    {
        raycastObjectMover = GetComponent<RaycastObjectMover>();
        ItemSelect(0); // 시작 시 1번 아이템 장착
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipRegisteredWand(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipRegisteredWand(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipRegisteredWand(2);
        }
    }

    // 등록된 지팡이를 장착하는 함수
    void EquipRegisteredWand(int index)
    {
        if (index < 0 || index >= registeredWands.Length)
        {
            Debug.LogWarning("잘못된 인덱스입니다.");
            return;
        }

        // 해당 슬롯에 등록된 지팡이가 있으면 장착
        if (registeredWands[index] != null)
        {
            // 이미 장착된 지팡이와 같은 지팡이인 경우에도 재장착 가능하도록 함
            if (currentEquippedItem != null)
            {
                currentEquippedItem.SetActive(false);
            }

            registeredWands[index].SetActive(true);
            currentEquippedItem = registeredWands[index];

            // 필요시 위치 및 회전 재조정
            ApplyItemTransform(currentEquippedItem);

            Debug.Log((index + 1) + "번 등록된 지팡이를 장착했습니다.");
        }
        else
        {
            // 등록된 지팡이가 없으면 기본 아이템 장착
            ItemSelect(index);
        }
    }

    void ItemSelect(int index)
    {
        if (index < 0 || index >= Item.Length)
        {
            Debug.LogWarning("아이템 인덱스가 잘못되었습니다.");
            return;
        }

        // 기존 장착된 아이템 비활성화
        if (currentEquippedItem != null)
        {
            currentEquippedItem.SetActive(false);
        }

        // 이미 해당 슬롯에 등록된 아이템이 있는지 확인
        if (registeredWands[index] == null)
        {
            // 새 아이템 생성 및 무기 슬롯에 장착
            GameObject newItem = Instantiate(Item[index], weaponslot);

            // WandItem 컴포넌트가 있는지 확인하고 적용
            ApplyItemTransform(newItem);

            // Animator나 Rigidbody 제거 (필요시)
            RemoveUnwantedComponents(newItem);

            currentEquippedItem = newItem;

            // 해당 슬롯에 아이템 등록
            registeredWands[index] = newItem;
        }
        else
        {
            // 이미 등록된 아이템이 있으면 활성화
            registeredWands[index].SetActive(true);
            currentEquippedItem = registeredWands[index];
        }
    }

    public void WearItem(GameObject hitObj)
    {
        if (hitObj == null)
        {
            Debug.LogWarning("장착할 오브젝트가 없습니다.");
            return;
        }

        // 기존 장착된 아이템 비활성화
        if (currentEquippedItem != null)
        {
            currentEquippedItem.SetActive(false);
        }

        // 이미 존재하는 오브젝트를 무기 슬롯으로 이동
        hitObj.transform.SetParent(weaponslot);

        // WandItem 컴포넌트가 있는지 확인하고 적용
        ApplyItemTransform(hitObj);
        RemoveUnwantedComponents(hitObj);

        // Ray로 선택한 지팡이를 숫자키 1, 2, 3에 순서대로 등록
        RegisterWandToNextSlot(hitObj);

        currentEquippedItem = hitObj;
    }

    // Ray로 선택한 지팡이를 다음 빈 슬롯에 등록하는 함수
    private void RegisterWandToNextSlot(GameObject wand)
    {
        // 이미 등록된 슬롯인지 확인
        for (int i = 0; i < registeredWands.Length; i++)
        {
            if (registeredWands[i] == wand)
            {
                Debug.Log("이미 " + (i + 1) + "번 슬롯에 등록된 지팡이입니다.");
                return;
            }
        }

        // 다음 빈 슬롯 찾기
        for (int i = 0; i < registeredWands.Length; i++)
        {
            if (registeredWands[i] == null)
            {
                registeredWands[i] = wand;
                Debug.Log("지팡이를 " + (i + 1) + "번 슬롯에 등록했습니다.");
                return;
            }
        }

        // 모든 슬롯이 차있으면 첫 번째 슬롯에 등록
        registeredWands[0] = wand;
        Debug.Log("모든 슬롯이 차있어 1번 슬롯에 지팡이를 덮어썼습니다.");
    }

    private void ApplyItemTransform(GameObject item)
    {
        // WandItem 컴포넌트가 있는지 확인
        WandItem wandItem = item.GetComponent<WandItem>();
        if (wandItem != null)
        {
            // WandItem에서 지정한 Transform 값을 적용
            item.transform.localPosition = wandItem.equipPosition;
            item.transform.localRotation = Quaternion.Euler(wandItem.equipRotation);
            item.transform.localScale = wandItem.equipScale;
            Debug.Log("지팡이 장착 완료 - 회전: " + wandItem.equipRotation + ", 크기: " + wandItem.equipScale);
        }
        else
        {
            // 기본 위치와 회전 적용
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
    }

    private void RemoveUnwantedComponents(GameObject item)
    {
        // 무기에 붙은 Animator 제거 (필요한 경우)
        Animator animator = item.GetComponent<Animator>();
        if (animator != null)
        {
            Destroy(animator);
        }

        // Rigidbody 제거 (물리 작용이 무기 장착 후 불필요한 경우)
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb);
        }

        // Collider 제거 (충돌로 인해 튕겨나가는 현상 방지)
        Collider col = item.GetComponent<Collider>();
        if (col != null)
        {
            Destroy(col);
        }
    }
}