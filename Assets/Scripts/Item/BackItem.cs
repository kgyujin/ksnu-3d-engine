using UnityEngine;

public class BackItem : MonoBehaviour
{
    [Header("�� ������ ����")]
    // Transform values for proper positioning when equipped on back
    // ĳ������ ���� ��ǥ�� ���� (-Z������ �� ����)
    public Vector3 equipPosition = new Vector3(0f, 1.0f, -1.0f); // �� �ڷ� 1m, ���� 1m
    public Vector3 equipRotation = new Vector3(0f, 0f, 0f); // �⺻ ȸ����
    public Vector3 equipScale = new Vector3(1f, 1f, 1f);

    [Header("������ ȿ��")]
    // Optional: Reference to any special effects or components
    public ParticleSystem itemEffect;
    public AudioSource equipSound;

    [Header("������ ����")]
    public string itemName = "Back Item";
    public string itemDescription = "� �����ϴ� �������Դϴ�.";

    // �������� Ȱ��ȭ�� �� ȣ��Ǵ� �޼���
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

        Debug.Log(itemName + " �������� Ȱ��ȭ�Ǿ����ϴ�.");
    }

    // �������� ��Ȱ��ȭ�� �� ȣ��Ǵ� �޼���
    public void DeactivateItem()
    {
        if (itemEffect != null)
        {
            itemEffect.Stop();
        }

        Debug.Log(itemName + " �������� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    // ������ ���� �� ȣ��Ǵ� �޼���
    public void OnEquip()
    {
        Debug.Log(itemName + " �������� � �����߽��ϴ�.");
        ActivateItem();
    }

    // ������ ���� �� ȣ��Ǵ� �޼���
    public void OnUnequip()
    {
        Debug.Log(itemName + " �������� ��� �����߽��ϴ�.");
        DeactivateItem();
    }
}