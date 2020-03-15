using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    [SerializeField] GameObject blockPrefab;
    [SerializeField] Sprite[] blockSprite;
    [SerializeField] GameObject scoreText;
    private int score;
    private Image background;
    private RawImage rawImage;

    private struct Block
    {
        public int x;
        public int y;
        public GameObject ob;

        public Block(int x, int y, GameObject ob)
        {
            this.x = x;
            this.y = y;
            this.ob = ob;
        }
    }


    private Block[] piece;
    private Block[] nextPiece;
    private int regularPieceCounter = 0;
    private int nextPieceNum;


    private int w = 10;
    private int h = 20;

    private float time = 0;
    private float dropSpeed = 0.4f;

    private Block[,] block;

    private int[,] shapes = new int[,]
    {
        {1, 3, 5, 7},
        {2, 4, 5, 7},
        {3, 4, 5, 6},
        {3, 4, 5, 7},
        {2, 3, 5, 7},
        {3, 5, 6, 7},
        {2, 3, 4, 5}
    };

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        scoreText.GetComponent<TextMeshProUGUI>().text = score.ToString();
        block = new Block[w, h];
        generateBlockPiece(Random.Range(0, shapes.GetLength(0)));
        regularPieceCounter++;
        nextPieceNum = Random.Range(0, shapes.GetLength(0));
        generateNextBlockPiece(nextPieceNum);
        GameObject.Find("AdsManager").gameObject.GetComponent<AdsController>().showBanner();
        StartCoroutine(showAdsWaitForSeconds(20f));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            movePiece(-1, 0);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            movePiece(1, 0);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            movePiece(0, -1);
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            rotatePiece();

        time += Time.deltaTime;
        if (time > dropSpeed)
        {
            if (!movePiece(0, -1))
            {
                for (int i = 0; i < piece.Length; i++)
                {
                    if (piece[i].y > 0)
                        GameObject.Find("LevelManager").gameObject.GetComponent<LevelManager>().LoadNextLevel();
                        //Debug.Log("gameover");
                    block[piece[i].x, -piece[i].y] = piece[i];
                }

                if (regularPieceCounter < 10)
                {
                    generateBlockPiece(nextPieceNum);
                    regularPieceCounter++;
                    nextPieceNum = Random.Range(0, shapes.GetLength(0));
                    generateNextBlockPiece(nextPieceNum);
                }
                else
                {
                    StartCoroutine(GameObject.Find("Background").GetComponent<BackgroundDownloader>().changeBackground());
                    generateHoledPiece();
                    regularPieceCounter = 0;
                }
                clear();
            }
            time = 0;
        }
    }

    private void generateBlockPiece(int nextPieceNum)
    {
        piece = new Block[4]
        {
            new Block(),
            new Block(),
            new Block(),
            new Block()
        };
        //int n = Random.Range(0, shapes.GetLength(0));
        //Sprite sprite = blockSprite[Random.Range(0, blockSprite.Length)];
        Sprite sprite = blockSprite[nextPieceNum];

        for (int i = 0; i < piece.Length; i++)
        {
            piece[i].x = (shapes[nextPieceNum, i] % 2) + 2;
            piece[i].y = (-shapes[nextPieceNum, i] / 2) + 4;

            piece[i].ob = Instantiate(blockPrefab, new Vector2(piece[i].x, piece[i].y), Quaternion.identity);
            SpriteRenderer sr = piece[i].ob.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;
        }
    }

    private void generateNextBlockPiece(int nextPieceNum)
    {
        if (nextPiece != null)
        {
            foreach (Block block in nextPiece)
                Destroy(block.ob);
        }
        nextPiece = new Block[4]
        {
            new Block(),
            new Block(),
            new Block(),
            new Block()
        };
        //int n = Random.Range(0, shapes.GetLength(0));
        //Sprite sprite = blockSprite[Random.Range(0, blockSprite.Length)];
        Sprite sprite = blockSprite[nextPieceNum];

        for (int i = 0; i < piece.Length; i++)
        {
            nextPiece[i].x = (shapes[nextPieceNum, i] % 2) + 2;
            nextPiece[i].y = (-shapes[nextPieceNum, i] / 2) + 4;

            nextPiece[i].ob = Instantiate(blockPrefab, new Vector2(nextPiece[i].x + 9, nextPiece[i].y - 4), Quaternion.identity);
            SpriteRenderer sr = piece[i].ob.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;
        }
    }

    private void generateHoledPiece()
    {
        int n = Random.Range(2, 4);
        bool leftSide = Random.value > 0.5f;
        int k = leftSide ? 0 : 5;
        List<Block> blocksToClear = new List<Block>();

        for (int i = h - 1; i > h - n; i--)
        {
            for(int g = k; g < k + 5; g++)
            {
                if (block[g, i].ob != null)
                    blocksToClear.Add(block[g,i]);
            }
        }

        if (blocksToClear.Count != 0)
        {
            piece = new Block[blocksToClear.Count];
            for (int i = 0; i < piece.Length; i++)
            {
                piece[i] = new Block();
                piece[i].x = blocksToClear[i].x;
                piece[i].y = blocksToClear[i].y + 19 - n;

                piece[i].ob = Instantiate(blockPrefab, new Vector2(piece[i].x, piece[i].y), Quaternion.identity);
                SpriteRenderer sr = piece[i].ob.GetComponent<SpriteRenderer>();
                sr.sprite = blocksToClear[i].ob.GetComponent<SpriteRenderer>().sprite;

                Destroy(blocksToClear[i].ob);
            }
        }
        else
            generateBlockPiece(nextPieceNum);
    }

    public void movePieceLeft()
    {
        movePiece(-1, 0);
    }

    public void movePieceRight()
    {
        movePiece(1, 0);
    }

    public void movePieceDown()
    {
        movePiece(0, -1);
    }

    private bool movePiece (int x, int y)
    {
        Block[] origin = piece.Clone() as Block[];

        for (int i = 0; i < piece.Length; i++)
        {
            piece[i].x += x;
            piece[i].y += y;
            //piece[i].ob.transform.position = new Vector2(piece[i].x, piece[i].y);
        }

        return setPiece(origin);
    }

    public void rotatePiece()
    {
        Block[] origin = piece.Clone() as Block[];
        Block p = piece[1];

        for (int i = 0; i < piece.Length; i++)
        {
            int x = piece[i].y - p.y;
            int y = piece[i].x - p.x;
            piece[i].x = p.x - x;
            piece[i].y = p.y + y;
            //piece[i].ob.transform.position = new Vector2(piece[i].x, piece[i].y);
        }

        setPiece(origin);
    }

    private bool setPiece(Block[] original)
    {
        bool set = true;

        for (int i = 0; i < piece.Length; i++)
        {
            try
            {
                if (piece[i].x < 0 || piece[i].x >= w || piece[i].y <= -h)
                    set = false;
                else if (block[piece[i].x, -piece[i].y].ob != null)
                    set = false;
            }
            catch
            { }
        }

        if (set)
            for (int i = 0; i < piece.Length; i++)
                piece[i].ob.transform.position = new Vector2(piece[i].x, piece[i].y);
        else
            piece = original;

        return set;
    }

    private void clear()
    {
        List<Block> blocksToClear = new List<Block>();
        int k = h - 1;
        int y = 0;

        for (int i = h -1; i > 0; i--)
        {
            blocksToClear.Clear();
            int count = 0;

            for (int j = 0; j < w; j++)
            {
                if (block[j, i].ob != null)
                    count++;

                block[j, i].y += y;
                blocksToClear.Add(block[j, i]);
                block[j, k] = block[j, i];
            }

            if (count < w)
                k--;
            else
            {
                y += -1;
                foreach (Block block in blocksToClear)
                {
                    Destroy(block.ob);
                }
                score += 100;
                scoreText.GetComponent<TextMeshProUGUI>().text = score.ToString();
            }

            for (int j = 0; j < w; j++)
            {
                if (block[j, i].ob != null)
                    block[j, i].ob.transform.position = new Vector2(block[j, i].x, block[j, i].y);
            }
        }
    }

    IEnumerator showAdsWaitForSeconds(float seconds)
    {
        while (true)
        {
            yield return new WaitForSeconds(seconds);
            StartCoroutine(GameObject.Find("adsManager").gameObject.GetComponent<AdsController>().showBanner());
        }
    }
}
