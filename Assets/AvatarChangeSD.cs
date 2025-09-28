using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarChangeSD : MonoBehaviour
{
    public Transform rootBone;
    public Transform faceTr;

    public Transform hairRootBone; // 이 캐릭터의 헤어는 Rootbone이 머리

    GameObject curFace;
    GameObject curEye;
    GameObject curCloth;
    GameObject curHair;

    public Button eye1;
    public Button eye2;

    public Button hair1;
    public Button hair2;
    
    public Button face1;
    public Button face2;

    public Button cloth1;
    public Button cloth2;

    Vector3 faceLocalPos = new Vector3(0.002f, -0.002f, -0.664f);

    public GameObject eye1_Model;
    public GameObject eye2_Model;
    public GameObject hair1_Model;
    public GameObject hair2_Model;
    public GameObject face1_Model;
    public GameObject face2_Model;
    public GameObject cloth1_Model;
    public GameObject cloth2_Model;


    private void Start()
    {
        //Debug.Log(cloth1_Model.GetComponentInChildren<SkinnedMeshRenderer>().bones.Length);
        //Debug.Log(hair1_Model.GetComponentInChildren<SkinnedMeshRenderer>().bones.Length);
        for(int i=0; i < hair1_Model.GetComponentInChildren<SkinnedMeshRenderer>().bones.Length; i++)
        {
            Debug.Log(hair2_Model.GetComponentInChildren<SkinnedMeshRenderer>().bones[i].name);
        }
        eye1.onClick.AddListener(() =>
        {
            RemoveEye();
            curEye = ChangeMesh(eye1_Model, rootBone, transform);
        });

        eye2.onClick.AddListener(() =>
        {
            RemoveEye();
            curEye = ChangeMesh(eye2_Model, rootBone, transform);
        });

        // 이 캐릭터의 헤어는 Rootbone이 머리이다. 
        hair1.onClick.AddListener(() =>
        {
            RemoveHair();
            //GameObject hair = Instantiate(hair1_Model, faceTr);

            curHair = ChangeMesh(hair1_Model, hairRootBone, hairRootBone);
        });

        hair2.onClick.AddListener(() =>
        {
            RemoveHair();
            //GameObject hair = Instantiate(hair2_Model, faceTr);

            curHair = ChangeMesh(hair2_Model, hairRootBone, hairRootBone);
        });

        // 이 캐릭터의 얼굴에는 스킨메쉬렌더러가 없다. 그냥 얹으면 된다.
        face1.onClick.AddListener(() =>
        {
            RemoveFace();
            GameObject face = Instantiate(face1_Model,faceTr);
            face.transform.localPosition = faceLocalPos;

            //if(curHair != null)
            //{ 
            //    curHair.GetComponent<SkinnedMeshRenderer>().rootBone = hairRootBone;
            //}
            curFace = face;
        });

        face2.onClick.AddListener(() =>
        {
            RemoveFace();
            GameObject face = Instantiate(face2_Model,faceTr);
            face.transform.localPosition = faceLocalPos;

            //if (curHair != null)
            //{ 
            //    curHair.GetComponent<SkinnedMeshRenderer>().rootBone = hairRootBone;
            //}

            curFace = face;
        });

        cloth1.onClick.AddListener(() =>
        {
            RemoveCloth();
            curCloth =  ChangeMesh(cloth1_Model, rootBone, transform);
        });

        cloth2.onClick.AddListener(() =>
        {
            RemoveCloth();
            curCloth = ChangeMesh(cloth2_Model, rootBone, transform);
        });
    }

    void RemoveFace()
    {
        if(curFace != null)
            Destroy(curFace);
    }

    void RemoveEye()
    {
        if (curEye != null)
            Destroy(curEye);
    }

    void RemoveCloth()
    {
        if (curCloth != null)
            Destroy(curCloth);
    }

    void RemoveHair()
    {
        if (curHair != null)
            Destroy(curHair);
    }

    GameObject ChangeMesh(GameObject prefab, Transform rootBone, Transform refTr)
    {
        GameObject part = prefab;

        SkinnedMeshRenderer[] smrs = part.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (smrs.Length > 0)
        {
            foreach (SkinnedMeshRenderer smr in smrs)
            {
                part = SetBone(smr, rootBone, refTr);
            }
        }

        return part;
    }

    GameObject SetBone(SkinnedMeshRenderer smr, Transform rootBone, Transform refTr)
    {
        GameObject part = new GameObject(smr.gameObject.name);
        part.transform.parent = this.transform;

        SkinnedMeshRenderer newSMR = part.AddComponent<SkinnedMeshRenderer>();

        Transform[] boneSet = new Transform[smr.bones.Length];

        for (int i = 0; i < smr.bones.Length; i++)
        {
            boneSet[i] = FindBoneByName(smr.bones[i].name, refTr);
        }

        newSMR.bones = boneSet;
        newSMR.rootBone = rootBone;
        newSMR.sharedMesh = smr.sharedMesh;
        // 이 캐릭터는 매터리얼이 여러개다!
        newSMR.materials = smr.sharedMaterials;
        //newSMR.material = smr.sharedMaterial;

        return part;
    }

    Transform FindBoneByName(string name, Transform refTransform)
    {
        if (refTransform.name == name)
        {
            return refTransform;
        }

        Transform returnTr;

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
