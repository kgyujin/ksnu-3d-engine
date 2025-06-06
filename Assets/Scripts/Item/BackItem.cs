using UnityEngine;

public class BackItem : MonoBehaviour
{
    [Header("등 아이템 설정")]
    // Transform values for proper positioning when equipped on back
    // 캐릭터의 로컬 좌표계 기준 (-Z방향이 등 방향)
    public Vector3 equipPosition = new Vector3(0f, 1.0f, -1.0f); // 등 뒤로 1m, 위로 1m
    public Vector3 equipRotation = new Vector3(0f, 0f, 0f); // 기본 회전값
    public Vector3 equipScale = new Vector3(1f, 1f, 1f);

    [Header("아이템 효과")]
    // Optional: Reference to any special effects or components
    public ParticleSystem itemEffect;
    public AudioSource equipSound;

    [Header("아이템 정보")]
    public string itemName = "Back Item";
    public string itemDescription = "등에 장착하는 아이템입니다.";

    // 아이템이 활성화될 때 호출되는 메서드
    public void ActivateItem()
    {
        if (itemEffect != null)
        {
            itemEffect.Play();
        }

        if (equipSound != null)
        {
            equipSound.Play();
        }

        Debug.Log(itemName + " 아이템이 활성화되었습니다.");
    }

    // 아이템이 비활성화될 때 호출되는 메서드
    public void DeactivateItem()
    {
        if (itemEffect != null)
        {
            itemEffect.Stop();
        }

        Debug.Log(itemName + " 아이템이 비활성화되었습니다.");
    }

    // 아이템 장착 시 호출되는 메서드
    public void OnEquip()
    {
        Debug.Log(itemName + " 아이템을 등에 장착했습니다.");
        ActivateItem();
    }

    // 아이템 해제 시 호출되는 메서드
    public void OnUnequip()
    {
        Debug.Log(itemName + " 아이템을 등에서 해제했습니다.");
        DeactivateItem();
    }
}