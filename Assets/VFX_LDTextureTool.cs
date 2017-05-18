using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TextureColorGroup
{
    public Color theColor;
    public GameObject theObject;
}



public class VFX_LDTextureTool : MonoBehaviour {

    [Header("Map Info")]
    public Texture2D theTexture;
    public Vector2 TextureSize;
    public Vector2 planeSize;

    [Header("Placement Options")]
    public bool useWorldGeo;
    public float rayHeight=50.0f;


    [Header("Objects")]
    public TextureColorGroup[] theGroups;



    GameObject thePlane;
    Material LDTextMaterial;
    Vector3 OriginPos
    { 
        get
        {
            return new Vector3(-planeSize.x / 2, 1, planeSize.y / 2);
        }
    }


    void OnDrawGizmos()
    {
        if (OriginPos != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.TransformPoint(OriginPos), 1);
        }
    }




    Vector3 GetPixelWorldCoord(Vector2 theCoord)
    {
        print("Checking Coord " + theCoord.ToString());
        float widthSegment = planeSize.x / TextureSize.x;
        float heightSegment = planeSize.y / TextureSize.y;

        float widthOffset = (widthSegment * theCoord.x) + widthSegment/2;
        float heightOffset= (heightSegment* theCoord.y) - planeSize.y+heightSegment/2;

        Vector3 thePos = new Vector3(OriginPos.x + widthOffset, 0, OriginPos.z + heightOffset);

        return thePos;
    }
    



    void CreateTempPlane()
    {
        thePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        thePlane.transform.SetParent(transform);
        thePlane.name = "Temp_Plane";
        thePlane.transform.localPosition = Vector3.zero;
        thePlane.transform.Rotate(new Vector3(0, 180, 0));
        // X = WIDTH
        // Z = HEIGHT
        thePlane.transform.localScale = new Vector3(planeSize.x/10,1,planeSize.y/10);

    }

    void CreateTempMaterial()
    {
        MeshRenderer MR = thePlane.GetComponent<MeshRenderer>();
        LDTextMaterial = Resources.Load("LDTempMaterial")as Material;
        MR.sharedMaterial = LDTextMaterial;
        MR.sharedMaterial.mainTexture = theTexture;
    }




#if UNITY_EDITOR
    void CreateGroupInstance(TextureColorGroup theGroup, Vector2 theCoord)
    {
        GameObject newObj = UnityEditor.PrefabUtility.InstantiatePrefab(theGroup.theObject as GameObject) as GameObject;

        newObj.transform.SetParent(transform);
        newObj.transform.localPosition = GetPixelWorldCoord(theCoord);

        if (useWorldGeo)
        {
            Ray ray = new Ray(new Vector3(newObj.transform.position.x, newObj.transform.position.y + rayHeight, newObj.transform.position.z),-Vector3.up);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayHeight + 10))
            { 
                newObj.transform.position = hit.point; 
            }

        }

    }

#endif

    [ContextMenu("Get Instances")]
    void GetGroupsInstances()
    {
        //print("getting Instances");

        if (theTexture != null)
        {
            //print("there is a texture");

            for (int i = 0; i < theGroups.Length; i++)
            {
                //print("checking group "+ i.ToString());

                for (int height = 0; height < TextureSize.y; height++)
                {
                    for (int width = 0; width < TextureSize.x; width++)
                    {
                        Color pixelColor = theTexture.GetPixel(width, height);
                        //print("Color at ["+width+","+height+"] "+pixelColor);
                        if (theGroups[i].theColor == pixelColor)
                        {
                            //Debug.Log("Found instance at ["+width+","+height+"]");
                            CreateGroupInstance(theGroups[i], new Vector2(width, height));

                        }
                    }
                }
            }
        }
    }

    [ContextMenu("Get Sizes")]
    void GetTextureSize()
    {
        if (theTexture != null)
        {
            TextureSize.x = theTexture.width;
            TextureSize.y = theTexture.height;
        }
    }

    [ContextMenu("Create Plane")]
    void CreatePlane()
    {
        CreateTempPlane();
        CreateTempMaterial();
    }
    
}
