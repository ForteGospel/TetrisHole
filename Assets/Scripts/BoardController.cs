using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    [SerializeField] GameObject blockPrefab;
    [SerializeField] Sprite[] blockSprite;
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
    private int regularPieceCounter = 0;


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
        block = new Block[w, h];
        generateBlockPiece();
        regularPieceCounter++;
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
                        Debug.Log("gameover");
                    block[piece[i].x, -piece[i].y] = piece[i];
                }

                if (regularPieceCounter < 10)
                {
                    generateBlockPiece();
                    regularPieceCounter++;
                }
                else
                {
                    generateHoledPiece();
                    regularPieceCounter = 0;
                }
                clear();
            }
            time = 0;
        }
    }

    private void generateBlockPiece()
    {
        piece = new Block[4]
        {
            new Block(),
            new Block(),
            new Block(),
            new Block()
        };
        int n = Random.Range(0, shapes.GetLength(0));
        Sprite sprite = blockSprite[Random.Range(0, blockSprite.Length)];

        for (int i = 0; i < piece.Length; i++)
        {
            piece[i].x = (shapes[n, i] % 2) + 2;
            piece[i].y = (-shapes[n, i] / 2) + 4;

            piece[i].ob = Instantiate(blockPrefab, new Vector2(piece[i].x, piece[i].y), Quaternion.identity);
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
            generateBlockPiece();
        

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
            }

            for (int j = 0; j < w; j++)
            {
                if (block[j, i].ob != null)
                    block[j, i].ob.transform.position = new Vector2(block[j, i].x, block[j, i].y);
            }
        }
    }

    IEnumerator ListObjectDestroyer(List<Block> blocksToClear)
    {
        foreach (Block block in blocksToClear)
        {
            block.ob.GetComponent<AnimatorController>().TriggerPopup();
            yield return new WaitForSeconds(1f);
            Destroy(block.ob);
        }
    }
}
