using UnityEngine;
using PassthroughCameraSamples;

public class PassthroughCameraDisplay : MonoBehaviour
{
    [SerializeField] private WebCamTextureManager webCamTextureManager;
    [SerializeField] private Renderer quadRenderer;
    [SerializeField] private string textureName;
    [SerializeField] private float quadDistance = 1;

    public bool isV1;

    private Texture2D picture;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        quadRenderer.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if(webCamTextureManager.WebCamTexture != null)
        {
            if (isV1)
            {
                quadRenderer.material.mainTexture = webCamTextureManager.WebCamTexture;
            }
            else
            {
                if (OVRInput.GetDown(OVRInput.Button.One))
                {
                    TakePicture();
                    PlaceQuad();
                }
            }

        }
    }

    private void TakePicture()
    {
        quadRenderer.gameObject.SetActive(true);

        int width = webCamTextureManager.WebCamTexture.width;
        int height = webCamTextureManager.WebCamTexture.height;

        if(picture == null)
        {
            picture = new Texture2D(width, height);
        }

        Color32[] pixels = new Color32[width * height];
        webCamTextureManager.WebCamTexture.GetPixels32(pixels);

        picture.SetPixels32(pixels);
        picture.Apply();

        quadRenderer.material.SetTexture(textureName, picture);
    }

    private void PlaceQuad()
    {
        Transform quadTransform = quadRenderer.transform; ;

        Pose cameraPose = PassthroughCameraUtils.GetCameraPoseInWorld(PassthroughCameraEye.Left);

        Vector2Int resolution = PassthroughCameraUtils.GetCameraIntrinsics(PassthroughCameraEye.Left).Resolution;

        quadTransform.position = cameraPose.position + cameraPose.forward * quadDistance;
        quadTransform.rotation = cameraPose.rotation;

        Ray leftSide = PassthroughCameraUtils.ScreenPointToRayInCamera(PassthroughCameraEye.Left, new Vector2Int(0, resolution.y / 2));
        Ray rightSide = PassthroughCameraUtils.ScreenPointToRayInCamera(PassthroughCameraEye.Right, new Vector2Int(resolution.x, resolution.y / 2));

        float horizontalPOV = Vector3.Angle(leftSide.direction, rightSide.direction);

        float quadScale = 2 * quadDistance *Mathf.Tan((horizontalPOV * Mathf.Deg2Rad) / 2);

        float ratio = picture.height / picture.width;

        quadTransform.localScale = new Vector3(quadScale, quadScale * ratio, 1);
    }
}
