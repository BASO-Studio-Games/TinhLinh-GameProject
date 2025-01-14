using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GPGManager : MonoBehaviour
{
    public TextMeshProUGUI DetailsText;
    [SerializeField] private Image profileImage; 
    // Start is called before the first frame update
    void Start()
    {
        SignIn();
    }

    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    public void ReSignIn()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }
    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services

            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string ImgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();


            DetailsText.text = "Success \n "+name + "\n" + id;

            if (!string.IsNullOrEmpty(ImgUrl))
            {
                // Bắt đầu tải ảnh từ URL
                StartCoroutine(LoadImageFromUrl(ImgUrl));
            }
            else
            {
                Debug.LogWarning("Image URL is empty.");
                DetailsText.text += "\n Url không tồn tại";
            }

        }
        else
        {
            DetailsText.text = "Sign in Failed!! " +  status;

            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }

    private IEnumerator LoadImageFromUrl(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Chuyển đổi Texture thành Sprite và gắn vào Image
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                profileImage.sprite = sprite;
            }
            else
            {
                Debug.LogError("Failed to load image: " + request.error);
                DetailsText.text += "\n Failed to load image: " + request.error;
            }
        }
    }
}
