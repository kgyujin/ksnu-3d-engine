using UnityEngine;

public class ItemSelectManager : MonoBehaviour
{
    [Header("아이템 설정")]
    public GameObject[] Item;            // 아이템 프리팹 배열

    [Header("장착 슬롯")]
    public Transform weaponslot;         // 손 본에 붙은 WeaponSlot (예: RightHand > WeaponSlot)
    public Transform backslot;           // 등 본에 붙은 BackSlot (예: Spine > BackSlot)

    private GameObject currentEquippedWeapon;
    private GameObject currentEquippedBackItem;
    private RaycastObjectMover raycastObjectMover;

    // 숫자키에 등록된 지팡이를 저장하는 배열
    private GameObject[] registeredWands = new GameObject[3];

    // 등 아이템을 저장하는 변수 (하나만 장착 가능하다고 가정)
    private GameObject registeredBackItem;

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
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // 4번키로 등 아이템 장착/해제
        {
            ToggleBackItem();
        }
    }

    void EquipRegisteredWand(int index)
    {
        if (index < 0 || index >= registeredWands.Length)
        {
            Debug.LogWarning("잘못된 인덱스입니다.");
            return;
        }

        if (registeredWands[index] != null)
        {
            if (currentEquippedWeapon != null)
            {
                currentEquippedWeapon.SetActive(false);
            }

            registeredWands[index].SetActive(true);
            currentEquippedWeapon = registeredWands[index];
            ApplyItemTransform(currentEquippedWeapon);
            Debug.Log((index + 1) + "번 등록된 지팡이를 장착했습니다.");
        }
        else
        {
            Debug.LogWarning((index + 1) + "번 슬롯에 등록된 지팡이가 없습니다. 기본 아이템 장착을 시도합니다.");

            // Item 배열에 아이템이 있으면 장착 시도
            if (index < Item.Length)
            {
                ItemSelect(index);
            }
            else
            {
                Debug.LogError("기본 아이템도 존재하지 않습니다. Item 배열 확인 필요.");
            }
        }
    }

    void ToggleBackItem()
    {
        if (registeredBackItem != null)
        {
            bool isActive = registeredBackItem.activeInHierarchy;
            registeredBackItem.SetActive(!isActive);
            currentEquippedBackItem = isActive ? null : registeredBackItem;
            Debug.Log("등 아이템을 " + (isActive ? "해제" : "장착") + "했습니다.");
        }
        else
        {
            Debug.LogWarning("등록된 등 아이템이 없습니다.");
        }
    }

    void ItemSelect(int index)
    {
        if (index < 0 || index >= Item.Length)
        {
            Debug.LogWarning("아이템 인덱스가 잘못되었습니다.");
            return;
        }

        // 기존 장착된 무기 아이템 비활성화
        if (currentEquippedWeapon != null)
        {
            currentEquippedWeapon.SetActive(false);
        }

        // 이미 해당 슬롯에 등록된 아이템이 있는지 확인
        if (registeredWands[index] == null)
        {
            // 새 아이템 생성 및 무기 슬롯에 장착
            GameObject newItem = Instantiate(Item[index], weaponslot);

            // WandItem 컴포넌트가 있는지 확인하고 적용
            ApplyItemTransform(newItem);

            // Animator나 Rigidbody 제거 (필요시)
            RemoveUnwantedComponents(newItem, false); // 무기는 콜라이더 제거

            currentEquippedWeapon = newItem;

            // 해당 슬롯에 아이템 등록
            registeredWands[index] = newItem;
        }
        else
        {
            // 이미 등록된 아이템이 있으면 활성화
            registeredWands[index].SetActive(true);
            currentEquippedWeapon = registeredWands[index];
        }
    }

    public void WearItem(GameObject hitObj)
    {
        if (hitObj == null)
        {
            Debug.LogWarning("장착할 오브젝트가 없습니다.");
            return;
        }

        // 아이템 타입 확인
        WandItem wandItem = hitObj.GetComponent<WandItem>();
        BackItem backItem = hitObj.GetComponent<BackItem>();

        if (backItem != null)
        {
            // 등 아이템 장착
            WearBackItem(hitObj);
        }
        else if (wandItem != null)
        {
            // 무기 아이템 장착
            WearWeaponItem(hitObj);
        }
        else
        {
            Debug.LogWarning("알 수 없는 아이템 타입입니다.");
        }
    }

    private void WearWeaponItem(GameObject hitObj)
    {
        // 기존 장착된 무기 비활성화
        if (currentEquippedWeapon != null)
        {
            currentEquippedWeapon.SetActive(false);
        }

        // 이미 존재하는 오브젝트를 무기 슬롯으로 이동
        hitObj.transform.SetParent(weaponslot);

        // WandItem 컴포넌트가 있는지 확인하고 적용
        ApplyItemTransform(hitObj);
        RemoveUnwantedComponents(hitObj, false); // 무기는 콜라이더 제거

        // Ray로 선택한 지팡이를 숫자키 1, 2, 3에 순서대로 등록
        RegisterWandToNextSlot(hitObj);

        currentEquippedWeapon = hitObj;
        Debug.Log("무기를 장착했습니다.");
    }

    private void WearBackItem(GameObject hitObj)
    {
        // 기존 등 아이템이 있으면 제거
        if (registeredBackItem != null)
        {
            Destroy(registeredBackItem);
        }

        // 캐릭터(this.transform)의 자식으로 설정
        hitObj.transform.SetParent(this.transform);

        // BackItem 컴포넌트 적용 (캐릭터 기준 등방향 위치 설정)
        ApplyBackItemTransform(hitObj);
        RemoveUnwantedComponents(hitObj, true); // 등 아이템은 콜라이더 유지

        registeredBackItem = hitObj;
        currentEquippedBackItem = hitObj;
        Debug.Log("등 아이템을 장착했습니다.");
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

    private void ApplyBackItemTransform(GameObject item)
    {
        // BackItem 컴포넌트가 있는지 확인
        BackItem backItem = item.GetComponent<BackItem>();
        if (backItem != null)
        {
            // BackItem에서 지정한 Transform 값을 적용 (캐릭터 로컬 좌표 기준)
            item.transform.localPosition = backItem.equipPosition;
            item.transform.localRotation = Quaternion.Euler(backItem.equipRotation);
            item.transform.localScale = backItem.equipScale;
            Debug.Log("등 아이템 장착 완료 - 위치: " + backItem.equipPosition + ", 회전: " + backItem.equipRotation);
        }
        else
        {
            // 기본 위치와 회전 적용 (등 뒤쪽으로 일정 거리)
            // 캐릭터의 로컬 좌표계에서 -Z방향이 등 방향
            item.transform.localPosition = new Vector3(0f, 1.0f, -1.0f); // 등 뒤로 1m, 위로 1m
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one;
        }
    }

    private void RemoveUnwantedComponents(GameObject item, bool keepCollider)
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

        // 콜라이더 제거 여부 결정 (등 아이템은 유지, 무기는 제거)
        if (!keepCollider)
        {
            Collider col = item.GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }
        }
    }
}