using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BackgroundDownloader : MonoBehaviour
{
    private SpriteRenderer background;
    private Page pageInfo;
    [SerializeField] Texture2D[] defaultBackgrounds;

    // Start is called before the first frame update
    void Start()
    {
        background = gameObject.GetComponent<SpriteRenderer>();
        setBackgroundFromDefault();
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
        {
            yield return new WaitForSeconds(10f);
            StartCoroutine(getPexelsPageInfo());
        }
        else
        {
            pageInfo = JsonConvert.DeserializeObject<Page>(request.downloadHandler.text);
            Debug.Log(pageInfo.PerPage + " " + pageInfo.TotalResults);
            yield return new WaitForSeconds(10f);
            StartCoroutine(changeBackground());
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
            setBackgroundFromDefault();
        else
        {
            Texture2D image = ((DownloadHandlerTexture)requestImage.downloadHandler).texture;
            //rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            background.sprite = Sprite.Create(image, new Rect(0f, 0f, image.width, image.height), Vector2.zero);
        }
    }
    

    // Update is called once per frame
    private void setBackgroundFromDefault()
    {
        int backgroundChoosen = Random.Range(0, defaultBackgrounds.Length);
        background.sprite = Sprite.Create(defaultBackgrounds[backgroundChoosen], new Rect(0f, 0f, defaultBackgrounds[backgroundChoosen].width, defaultBackgrounds[backgroundChoosen].height), Vector2.zero);
    }
}
