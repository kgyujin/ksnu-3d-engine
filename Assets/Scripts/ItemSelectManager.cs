using UnityEngine;

public class ItemSelectManager : MonoBehaviour
{
    public GameObject[] Item;            // 아이템 프리팹 배열
    public Transform weaponslot;         // 손 본에 붙은 WeaponSlot (예: RightHand > WeaponSlot)
    private GameObject currentEquippedItem;
    private RaycastObjectMover raycastObjectMover;

    void Start()
    {
        raycastObjectMover = GetComponent<RaycastObjectMover>();
        ItemSelect(0); // 시작 시 1번 아이템 장착
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ItemSelect(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ItemSelect(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ItemSelect(2);
        }
    }

    void ItemSelect(int index)
    {
        if (index < 0 || index >= Item.Length)
        {
            Debug.LogWarning("아이템 인덱스가 잘못되었습니다.");
            return;
        }

        // 기존 장착된 아이템 제거
        if (currentEquippedItem != null)
        {
            Destroy(currentEquippedItem);
        }

        // 새 아이템 생성 및 무기 슬롯에 장착
        GameObject newItem = Instantiate(Item[index], weaponslot);

        // WandItem 컴포넌트가 있는지 확인하고 적용
        ApplyItemTransform(newItem);

        // Animator나 Rigidbody 제거 (필요시)
        RemoveUnwantedComponents(newItem);
        currentEquippedItem = newItem;
    }

    public void WearItem(GameObject hitObj)
    {
        if (hitObj == null)
        {
            Debug.LogWarning("장착할 오브젝트가 없습니다.");
            return;
        }

        // 기존 장착된 아이템 제거
        if (currentEquippedItem != null)
        {
            Destroy(currentEquippedItem);
        }

        // 이미 존재하는 오브젝트를 무기 슬롯으로 이동
        hitObj.transform.SetParent(weaponslot);

        // WandItem 컴포넌트가 있는지 확인하고 적용
        ApplyItemTransform(hitObj);

        RemoveUnwantedComponents(hitObj);
        currentEquippedItem = hitObj;
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
