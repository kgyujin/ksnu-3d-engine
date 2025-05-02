using UnityEngine;
using System.Collections.Generic;

public class WeightChecker : MonoBehaviour
{
    public float pressMinMass = 20f;

    private Foothold foothold;
    private Door door;

    private HashSet<Rigidbody> trackedBodies = new HashSet<Rigidbody>();
    private bool isPressed = false;

    private void Awake()
    {
        foothold = GetComponent<Foothold>();
        door = GetComponentInChildren<Door>();

        Debug.Log("[WeightChecker] �ʱ�ȭ �Ϸ�");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null)
        {
            trackedBodies.Add(rb);
            Debug.Log($"[�浹 ����] �߰���: {rb.name}, ���� = {rb.mass}");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null)
        {
            trackedBodies.Remove(rb);
            Debug.Log($"[�浹 ����] ���ŵ�: {rb.name}, ���� = {rb.mass}");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // ���������� �浹 ���¸� �����ϰ� ��ü�� ���� �ִ��� Ȯ��
        Rigidbody rb = collision.rigidbody;
        if (rb != null && !trackedBodies.Contains(rb))
        {
            trackedBodies.Add(rb);
            Debug.Log($"[�浹 ����] �߰���: {rb.name}, ���� = {rb.mass}");
        }
    }

    private void FixedUpdate()
    {
        float totalMass = 0f;

        // ���� �浹 ���� ��� ������ٵ��� ������ �ջ�
        foreach (var rb in trackedBodies)
        {
            if (rb != null)
                totalMass += rb.mass;
        }

        Debug.Log($"[���� ����] ���� �� ����: {totalMass} / �Ӱ谪: {pressMinMass}");

        // ������ �Ӱ谪 �̻��̸� ������ �����ְ� ���� ���ϴ�
        if (totalMass >= pressMinMass && !isPressed)
        {
            foothold.Press();
            door.Open();
            isPressed = true;
            Debug.Log($"[���� ��ȭ] �Ӱ谪 ���� �� ���� ���� & �� ���� (�� ����: {totalMass})");
        }
        // �Ӱ谪 �̸��̸� ������ ����ġ�ϰ� ���� �ݽ��ϴ�
        else if (totalMass < pressMinMass && isPressed)
        {
            foothold.Release();
            door.Close();
            isPressed = false;
            Debug.Log($"[���� ��ȭ] �Ӱ谪 �̴� �� ���� ����ġ & �� ���� (�� ����: {totalMass})");
        }
    }
}
