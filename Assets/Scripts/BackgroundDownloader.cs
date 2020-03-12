using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BackgroundDownloader : MonoBehaviour
{
    private SpriteRenderer background;
    private Page pageInfo;
    // Start is called before the first frame update
    void Start()
    {
        background = gameObject.GetComponent<SpriteRenderer>();
        StartCoroutine(getPexelsPageInfo());
    }

    IEnumerator getPexelsPageInfo()
    {
        string MediaUrl = "https://api.pexels.com/v1/search?query=erotic&per_page=80";
        UnityWebRequest request = UnityWebRequest.Get(MediaUrl);
        //UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        request.SetRequestHeader("Authorization", "563492ad6f917000010000011fdbc331304044c7a77a9734d2988ab2");
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            pageInfo = JsonConvert.DeserializeObject<Page>(request.downloadHandler.text);
        }
    }

    public IEnumerator changeBackground()
    {
        int numOfPages = pageInfo.TotalResults / pageInfo.PerPage;
        pageInfo.PageNumber = Random.Range(1, numOfPages - 2);
        UnityWebRequest requestImage = UnityWebRequestTexture.GetTexture(pageInfo.Photos[Random.Range(0, pageInfo.PerPage - 1)].Src.Portrait);
        requestImage.SetRequestHeader("Authorization", "563492ad6f917000010000011fdbc331304044c7a77a9734d2988ab2");
        yield return requestImage.SendWebRequest();
        if (requestImage.isNetworkError || requestImage.isHttpError)
            Debug.Log(requestImage.error);
        else
        {
            Texture2D image = ((DownloadHandlerTexture)requestImage.downloadHandler).texture;
            //rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            background.sprite = Sprite.Create(image, new Rect(0f, 0f, image.width, image.height), Vector2.zero);
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(changeBackground());
    }
}
