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

        if (currentEquippedItem != null)
        {
            Destroy(currentEquippedItem);
        }

        // 새 아이템 인스턴스화 후 무기 슬롯에 장착
        GameObject newItem = Instantiate(Item[index]);
        newItem.transform.SetParent(weaponslot, false); // 부모 설정

        // WandItem에서 transform 값 받아 적용
        WandItem wandItem = newItem.GetComponentInChildren<WandItem>();
        if (wandItem != null)
        {
            newItem.transform.localPosition = wandItem.localPosition;
            newItem.transform.localRotation = Quaternion.Euler(wandItem.localRotation);
            newItem.transform.localScale = wandItem.localScale;
        }
        else
        {
        newItem.transform.localPosition = Vector3.zero;
        newItem.transform.localRotation = Quaternion.identity;
            newItem.transform.localScale = Vector3.one;
        }

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

        if (currentEquippedItem != null)
        {
            Destroy(currentEquippedItem);
        }

        // 이미 존재하는 오브젝트를 무기 슬롯으로 이동
        hitObj.transform.SetParent(weaponslot);
        hitObj.transform.localPosition = Vector3.zero;
        hitObj.transform.localRotation = Quaternion.identity;

        RemoveUnwantedComponents(hitObj);
        currentEquippedItem = hitObj;
    }

    private void RemoveUnwantedComponents(GameObject item)
    {
        Animator animator = item.GetComponent<Animator>();
        if (animator != null)
        {
            Destroy(animator);
        }

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
