using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarChange : MonoBehaviour
{
    [Header("옷 갈아입기 버튼 GUI")]
    public Button nextBtn;
    public Button prevBtn;

    [Header("가장 상위 본(반드시 의상의 Skinned Mesh Renderer의 Root Bone과 동일하게!)")]
    public Transform  rootBone;

    [Header("캐릭터의 의상 리스트")]
    public GameObject[] clothArray;
    
    int index = -1; // 현재 입고 있는 의상 배열의 인덱스

    public Transform clothRoot; // 갈아입은 의상을 어떤 트랜스폼 하위에 둘 것인가?

    List<GameObject> curCloth;  // 현재 입고 있는 의상 (오브젝트가 여러개 일 수 있음)

    private void Start()
    {
        curCloth = new List<GameObject>();

        nextBtn.onClick.AddListener(() =>
        {
            index++;
            // 인덱스가 배열의 길이를 초과하면 초기화
            if (index >= clothArray.Length)
                index = 0;

            ChangeMesh(clothArray[index]);
        });

        prevBtn.onClick.AddListener(() =>
        {
            index--;
            // 인덱스가 0 미만으로 떨어지면 초기화
            if (index < 0)
                index = clothArray.Length - 1;

            ChangeMesh(clothArray[index]);
        });
    }

    void RemoveCloth() // 현재 입고 있는 의상 벗기
    {
        for(int i =0; i <curCloth.Count; i++)
            Destroy(curCloth[i]);
    }

    GameObject ChangeMesh(GameObject prefab)
    {
        // 우선 입고 있는 의상을 제거한다. 처음 옷을 입을 때는 입고 있는 의상이 없으니 예외처리!
        if (curCloth != null)
            RemoveCloth();

        GameObject part = prefab; // 입으려는 의상의 Prefab을 매개변수로 받아온다.

        // 입으려는 의상의 모든 스킨메쉬렌더러를 가져온다. (의상의 오브젝트가 여러개일 수 있기 때문에 배열로 받아온다.)
        SkinnedMeshRenderer[] smrs = part.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (smrs.Length > 0)
        {
            // 모든 스킨메쉬렌더러의 Bone 정보를 이 캐릭터의 Bone과 일치시켜준다.
            foreach (SkinnedMeshRenderer smr in smrs)
            {
                part = SetBone(smr);
            }
        }

        return part;
    }

    // 메쉬 렌더러의 모든 Bone 정보를 조회한다.
    GameObject SetBone(SkinnedMeshRenderer smr)
    {
        // 1. 여기서 입으려는 의상의 오브젝트가 생성된다.
        // 2. 완전 새로운 빈오브젝트를 생성한다.
        // 3. 일단 이름만 먼저 설정한다.
        GameObject part = new GameObject(smr.gameObject.name);
        // 의상을 보기 좋게 하나의 오브젝트 하위로 보낸다.
        part.transform.parent = clothRoot;
        // 현재 입고 있는 의상목록에 기록한다.
        curCloth.Add(part);

        // 생성한 빈 오브젝트에 스킨메쉬렌더러를 추가해준다.
        SkinnedMeshRenderer newSMR = part.AddComponent<SkinnedMeshRenderer>();

        // 입으려는 의상모델의 Bone 숫자만큼 Transform을 생성
        Transform[] boneSet = new Transform[smr.bones.Length];

        // 입으려는 의상모델의 모든 Bone 정보를 조회해 현재 캐릭터의 Bone 셋팅과 일치하도록 설정한다.
        for (int i = 0; i < smr.bones.Length; i++)
        {
            // [골반] [척추1] [척추2] [척추3] [어깨] ... 이런식으로 본의 순서가 맞아떨어지도록 설정하는 작업
            boneSet[i] = FindBoneByName(smr.bones[i].name, transform);
        }

        newSMR.bones = boneSet;
        newSMR.rootBone = rootBone;
        newSMR.sharedMesh = smr.sharedMesh;
        // Material이 여러개일 수도 있음! 이럴땐 모든 매터리얼을 가져오도록
        //newSMR.materials = smr.sharedMaterials;
        newSMR.material = smr.sharedMaterial;

        return part;
    }

    Transform FindBoneByName(string name, Transform refTransform)
    {
        // 내 본의 이름과 일치할 경우...
        if (refTransform.name == name)
        {
            return refTransform;
        }

        Transform returnTr;
        // transform 하위에는 자식들의 transform 정보가 모두 담겨 있다. 
        // transform.transform.transform.transform.transform.transform.transform

        // 이 캐릭터의 하위 transform을 모두 돌면서 이름이 매칭되는 Bone을 찾아낸다.
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
