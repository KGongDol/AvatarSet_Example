using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarChange : MonoBehaviour
{
    [Header("�� �����Ա� ��ư GUI")]
    public Button nextBtn;
    public Button prevBtn;

    [Header("���� ���� ��(�ݵ�� �ǻ��� Skinned Mesh Renderer�� Root Bone�� �����ϰ�!)")]
    public Transform  rootBone;

    [Header("ĳ������ �ǻ� ����Ʈ")]
    public GameObject[] clothArray;
    
    int index = -1; // ���� �԰� �ִ� �ǻ� �迭�� �ε���

    public Transform clothRoot; // �������� �ǻ��� � Ʈ������ ������ �� ���ΰ�?

    List<GameObject> curCloth;  // ���� �԰� �ִ� �ǻ� (������Ʈ�� ������ �� �� ����)

    private void Start()
    {
        curCloth = new List<GameObject>();

        nextBtn.onClick.AddListener(() =>
        {
            index++;
            // �ε����� �迭�� ���̸� �ʰ��ϸ� �ʱ�ȭ
            if (index >= clothArray.Length)
                index = 0;

            ChangeMesh(clothArray[index]);
        });

        prevBtn.onClick.AddListener(() =>
        {
            index--;
            // �ε����� 0 �̸����� �������� �ʱ�ȭ
            if (index < 0)
                index = clothArray.Length - 1;

            ChangeMesh(clothArray[index]);
        });
    }

    void RemoveCloth() // ���� �԰� �ִ� �ǻ� ����
    {
        for(int i =0; i <curCloth.Count; i++)
            Destroy(curCloth[i]);
    }

    GameObject ChangeMesh(GameObject prefab)
    {
        // �켱 �԰� �ִ� �ǻ��� �����Ѵ�. ó�� ���� ���� ���� �԰� �ִ� �ǻ��� ������ ����ó��!
        if (curCloth != null)
            RemoveCloth();

        GameObject part = prefab; // �������� �ǻ��� Prefab�� �Ű������� �޾ƿ´�.

        // �������� �ǻ��� ��� ��Ų�޽��������� �����´�. (�ǻ��� ������Ʈ�� �������� �� �ֱ� ������ �迭�� �޾ƿ´�.)
        SkinnedMeshRenderer[] smrs = part.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (smrs.Length > 0)
        {
            // ��� ��Ų�޽��������� Bone ������ �� ĳ������ Bone�� ��ġ�����ش�.
            foreach (SkinnedMeshRenderer smr in smrs)
            {
                part = SetBone(smr);
            }
        }

        return part;
    }

    // �޽� �������� ��� Bone ������ ��ȸ�Ѵ�.
    GameObject SetBone(SkinnedMeshRenderer smr)
    {
        // 1. ���⼭ �������� �ǻ��� ������Ʈ�� �����ȴ�.
        // 2. ���� ���ο� �������Ʈ�� �����Ѵ�.
        // 3. �ϴ� �̸��� ���� �����Ѵ�.
        GameObject part = new GameObject(smr.gameObject.name);
        // �ǻ��� ���� ���� �ϳ��� ������Ʈ ������ ������.
        part.transform.parent = clothRoot;
        // ���� �԰� �ִ� �ǻ��Ͽ� ����Ѵ�.
        curCloth.Add(part);

        // ������ �� ������Ʈ�� ��Ų�޽��������� �߰����ش�.
        SkinnedMeshRenderer newSMR = part.AddComponent<SkinnedMeshRenderer>();

        // �������� �ǻ���� Bone ���ڸ�ŭ Transform�� ����
        Transform[] boneSet = new Transform[smr.bones.Length];

        // �������� �ǻ���� ��� Bone ������ ��ȸ�� ���� ĳ������ Bone ���ð� ��ġ�ϵ��� �����Ѵ�.
        for (int i = 0; i < smr.bones.Length; i++)
        {
            // [���] [ô��1] [ô��2] [ô��3] [���] ... �̷������� ���� ������ �¾ƶ��������� �����ϴ� �۾�
            boneSet[i] = FindBoneByName(smr.bones[i].name, transform);
        }

        newSMR.bones = boneSet;
        newSMR.rootBone = rootBone;
        newSMR.sharedMesh = smr.sharedMesh;
        // Material�� �������� ���� ����! �̷��� ��� ���͸����� ����������
        //newSMR.materials = smr.sharedMaterials;
        newSMR.material = smr.sharedMaterial;

        return part;
    }

    Transform FindBoneByName(string name, Transform refTransform)
    {
        // �� ���� �̸��� ��ġ�� ���...
        if (refTransform.name == name)
        {
            return refTransform;
        }

        Transform returnTr;
        // transform �������� �ڽĵ��� transform ������ ��� ��� �ִ�. 
        // transform.transform.transform.transform.transform.transform.transform

        // �� ĳ������ ���� transform�� ��� ���鼭 �̸��� ��Ī�Ǵ� Bone�� ã�Ƴ���.
        foreach (Transform tr in refTransform)
        {
            returnTr = FindBoneByName(name, tr);
            if (returnTr != null)
            {
                return returnTr;
            }
        }

        return null;
    }

}
