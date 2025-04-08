using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;

public class RandomPathGenerator : MonoBehaviour
{
    [Header("Kích thước sàn đấu:")]//----------
    [SerializeField] private int rows = 7;
    [SerializeField] private int cols = 11;

    [Header("Điểm bắt đầu và điểm kết thúc:")]//----------
    [Tooltip("Điểm bắt đầu là vị trí quái sẽ đi vào.")]
    [SerializeField] private Vector2Int start = new Vector2Int(3, 10);

    [Tooltip("Điểm kết thúc là vị trí ta cần bảo vệ.")]
    [SerializeField] private Vector2Int end = new Vector2Int(0, 9);

    [Tooltip("Số bước đi tối đa mà thuật toán tạo ra.")]
    [SerializeField] private int maxSteps = 20;

    [Header("Thuộc tính đường đi:")]//----------
    [Tooltip("GameObject cha để chứa các Prefab.")]
    [SerializeField] private GameObject parentObject;

    [Tooltip("Prefab đại diện cho ô vuông: 0-Đường thẳng; 1-Đường cua; 2-Ngoài viền")]
    [SerializeField] private GameObject[] tilePrefab;

    [Tooltip("Kích thước của mỗi ô vuông.")]
    [SerializeField] private float tileSize = 130.0f;

    [Header("Mã nguồn liên quan:")]//----------
    [SerializeField] private LevelManager levelManager;

    private bool[,] visited; // Các điểm đã được duyệt qua
    private List<Vector2Int> path; // Đường đi được tạo

    [Header("Nút bấm:")]//----------
    [SerializeField] private Button rollButton;
    [SerializeField] private Button startButton;

    [Header("Bắt đầu trò chơi:")]//----------
    [SerializeField] private GameObject UIScreen;

    [SerializeField] public bool isTutorial;

    // Firebase
    private DatabaseReference dbReference;
    private int currentRollsCount; // Số roll hiện tại của người chơi
    private string userID;
    private TMP_Text rollCountText;

    private void Start()
    {
        // Thiết lập đường đi
        visited = new bool[rows, cols];
        path = new List<Vector2Int>();

        UIScreen.SetActive(false);

        if (isTutorial){
            rollButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(false);
        }
        else{
            rollButton.onClick.AddListener(CheckAndRoll);
            rollCountText = rollButton.GetComponentInChildren<TMP_Text>();
            
            startButton.onClick.AddListener(StartGame);
        }

        ResetPath();

        userID = PlayerPrefs.GetString("ID_User", "DefaultUserID");
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        LoadUserRolls();
    }

    /// <summary>
    /// Tạo đường dẫn từ vị trí hiện tại đến điểm kết thúc trên bản đồ.
    /// </summary>
    /// <param name="current">
    /// Vị trí hiện tại (tọa độ dạng Vector2Int) trong lưới.
    /// </param>
    /// <returns>
    /// Trả về `true` nếu tìm thấy đường dẫn đến điểm kết thúc, 
    /// ngược lại trả về `false`.
    /// </returns>
    private bool GeneratePath(Vector2Int current)
    {
        path.Add(current);
        visited[current.x, current.y] = true;

        if (current == end)
            return true;

        List<Vector2Int> neighbors = GetNeighbors(current);
        Shuffle(neighbors);

        foreach (Vector2Int neighbor in neighbors)
        {
            if (!visited[neighbor.x, neighbor.y])
            {
                if (GeneratePath(neighbor))
                    return true; // Tìm được đường, kết thúc
            }
        }

        path.RemoveAt(path.Count - 1); // Backtrack
        return false;
    }

    public void GenerateStraightPath()
    {
        path.Clear(); // Xóa đường cũ nếu có

        int step = (start.y < end.y) ? 1 : -1; // Xác định hướng đi

        for (int y = start.y; y != end.y + step; y += step)
        {
            path.Add(new Vector2Int(start.x, y));
        }
    }

    // Lấy danh sách ô hàng xóm ngẫu nhiên
    private List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions = {
            new Vector2Int(0, 1), // Right
            new Vector2Int(0, -1), // Left
            new Vector2Int(1, 0), // Down
            new Vector2Int(-1, 0) // Up
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = pos + dir;
            if (neighbor.x >= 0 && neighbor.x < rows && neighbor.y >= 0 && neighbor.y < cols)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    // Xáo trộn danh sách
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void DisplayPath()
    {
        // Tạo ma trận lưu các đối tượng backgroundTile
        GameObject[,] backgroundTiles = new GameObject[rows, cols];

        // Lấp đầy ma trận
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 position = new Vector3(col * tileSize, -row * tileSize, 0);
                GameObject backgroundTile = Instantiate(tilePrefab[2], position, Quaternion.identity);
                backgroundTile.transform.SetParent(parentObject.transform, false);

                // Lưu đối tượng vào ma trận
                backgroundTiles[row, col] = backgroundTile;

                // Đặt kích thước của ô vuông
                RectTransform rectTransform = backgroundTile.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.sizeDelta = new Vector2(tileSize, tileSize);
                }
            }
        }

        // Biến để theo dõi số thứ tự
        int stepNumber = 1;

        // Duyệt qua danh sách đường đi
        for (int i = 0; i < path.Count; i++)
        {
            Vector2Int step = path[i];

            // Nếu có backgroundTile ở vị trí này, xóa nó
            if (backgroundTiles[step.x, step.y] != null)
            {
                Destroy(backgroundTiles[step.x, step.y]);
                backgroundTiles[step.x, step.y] = null; // Đặt giá trị về null để tránh xóa lại
            }

            // Tính toán vị trí của ô vuông trong Canvas
            Vector3 position = new Vector3(step.y * tileSize, -step.x * tileSize, 0);
            GameObject tile;
            Quaternion rotation = Quaternion.identity;
            int movementStatus = CheckMovement(i);

            switch (movementStatus)
            {
                case 0:
                    rotation = Quaternion.Euler(0, 0, 90);
                    tile = Instantiate(tilePrefab[0], position, rotation);
                    break;
                case 1:
                    rotation = Quaternion.Euler(0, 0, 0);
                    tile = Instantiate(tilePrefab[0], position, rotation);
                    break;
                case 2:
                    rotation = Quaternion.Euler(0, 0, 270);
                    tile = Instantiate(tilePrefab[1], position, rotation);
                    break;
                case 3:
                    rotation = Quaternion.Euler(0, 0, 0);
                    tile = Instantiate(tilePrefab[1], position, rotation);
                    break;
                case 4:
                    rotation = Quaternion.Euler(0, 0, 180);
                    tile = Instantiate(tilePrefab[1], position, rotation);
                    break;
                case 5:
                    rotation = Quaternion.Euler(0, 0, 90);
                    tile = Instantiate(tilePrefab[1], position, rotation);
                    break;
                default:
                    rotation = Quaternion.Euler(0, 0, 90);
                    tile = Instantiate(tilePrefab[1], position, rotation);
                    break;
            }

            tile.gameObject.name = "Tile - " + stepNumber;
            tile.transform.SetParent(parentObject.transform, false); // false đảm bảo không thay đổi tỷ lệ (scale)

            // Gán điểm quái di chuyển
            if (i == 0){
                // Tạo một GameObject mới
                GameObject newObject = new GameObject("StartPoint");
                newObject.transform.SetParent(parentObject.transform, false);

                // Đặt vị trí của GameObject mới cách tile 5 đơn vị về phía bên phải
                newObject.transform.position = tile.transform.position + new Vector3(5f, 0f, 0f);

                // Cập nhật levelManager với transform của GameObject mới
                levelManager.startPoint = newObject.transform;
            }

            if (movementStatus > 1) // Không phải đi thẳng
                levelManager.AddNewTransformTilePoint(tile.transform);

            if (i == path.Count - 1){
                // Tạo một GameObject mới
                GameObject newObject = new GameObject("EndPoint");
                newObject.transform.SetParent(parentObject.transform, false);

                // Đặt vị trí của GameObject mới cách tile 1 đơn vị về phía bên trái
                newObject.transform.position = tile.transform.position + new Vector3(-1f, 0f, 0f);
                levelManager.AddNewTransformTilePoint(newObject.transform);
            }

            // Lấy RectTransform của đối tượng tạo ra
            RectTransform rectTransform = tile.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(tileSize, tileSize); // Đặt kích thước ô vuông
            }

            stepNumber++;
        }
    }

    /// <summary>
    /// Hàm kiểm tra một bước có phải là khúc cua hay không, và nếu có thì là cua trái hay cua phải
    /// </summary>
    /// <param name="index">
    /// Vị trí hiện tại (tọa độ dạng Vector2Int) trong lưới.
    /// </param>
    /// <returns>
    /// 0: Đi ngang; 1: Đi dọc; 
    /// 2: Đi xuống, cua sang phải OR Sang phải, cua lên trên; 
    /// 3: Đi xuống, cua sang trái OR Sang trái, cua lên trên;
    /// 4: Đi lên, cua sang phải OR Sang phải, cua xuống dưới;
    /// 5: Đi lên, cua sang trái OR Sang trái, cua xuống dưới.
    /// </returns>
    private int CheckMovement(int index)
    {
        // Lấy điểm trước, hiện tại, và sau
        Vector2Int prevPoint;
        Vector2Int currentPoint = path[index];
        Vector2Int nextPoint;

        // Nếu là điểm đầu hoặc điểm cuối thì thêm hai điểm ẩn
        if (index == 0)
            prevPoint = new Vector2Int(currentPoint.x, currentPoint.y + 1);
        else
            prevPoint = path[index - 1];
        
        if (index == path.Count - 1)
            nextPoint = new Vector2Int(currentPoint.x, currentPoint.y - 1);
        else
            nextPoint = path[index + 1];
        

        // Tính vector hướng
        Vector2Int prevDirection = currentPoint - prevPoint; // Vector từ điểm trước đến điểm hiện tại
        Vector2Int nextDirection = nextPoint - currentPoint; // Vector từ điểm hiện tại đến điểm sau

        // Kiểm tra đi ngang hay đi dọc
        if (prevDirection.x == 0 && nextDirection.x == 0)
        {
            // Debug.Log($"Index {index}: Đi ngang.");
            return 0;
        }
        else if (prevDirection.y == 0 && nextDirection.y == 0)
        {
            // Debug.Log($"Index {index}: Đi dọc.");
            return 1;
        }
        // Kiểm tra khúc cua
        else
        {
            if ((prevDirection.x > 0 && nextDirection.y > 0) || (prevDirection.y < 0 && nextDirection.x < 0))
            {
                // Debug.Log($"Index {index}: Xuống cua qua phải."); // Đi xuống, cua sang phải
                // Debug.Log($"Index {index}: Phải cua lên trên."); // Sang phải, cua lên trên
                return 2;
            }
            else if ((prevDirection.x > 0 && nextDirection.y < 0) || (prevDirection.y > 0 && nextDirection.x < 0))
            {
                // Debug.Log($"Index {index}: Xuống cua qua trái."); // Đi xuống, cua sang trái
                // Debug.Log($"Index {index}: Trái cua lên trên."); // Sang trái, cua lên trên
                return 3;
            }
            else if ((prevDirection.x < 0 && nextDirection.y > 0) || (prevDirection.y < 0 && nextDirection.x > 0))
            {
                // Debug.Log($"Index {index}: Lên cua qua phải."); // Đi lên, cua sang phải
                // Debug.Log($"Index {index}: Phải cua xuống dưới."); // Sang phải, cua xuống dưới
                return 4;
            }
            else if ((prevDirection.x < 0 && nextDirection.y < 0) || (prevDirection.y > 0 && nextDirection.x > 0))
            {
                // Debug.Log($"Index {index}: Lên cua qua trái."); // Đi lên, cua sang trái
                // Debug.Log($"Index {index}: Trái cua xuống dưới."); // Sang trái, cua xuống dưới
                return 5;
            }
        }

        return 0;
    }

    /// <summary>
    /// Xóa đường đi cũ đi và tạo lại đường đi mới.
    /// </summary>
    public void ResetPath()
    {
        do
        {
            // Xóa tất cả các đối tượng tile cũ trong parentObject
            foreach (Transform child in parentObject.transform)
            {
                Destroy(child.gameObject);
            }
            
            levelManager.path = new Transform[0];
            

            // Đặt lại trạng thái visited và path
            visited = new bool[rows, cols];
            path.Clear();

            // Tạo lại đường đi
            if (isTutorial)
                GenerateStraightPath();
            else
                GeneratePath(start);

        } while (path.Count > maxSteps); // Lặp lại nếu số bước vượt quá maxSteps

        
        DisplayPath();
    }

    // void Update()
    // {
    //     // Khi nhấn phím Space, tạo lại bản đồ
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         ResetPath(); 
    //     }
    // }

    private void LoadUserRolls()
    {
        dbReference.Child("Users").Child(userID).Child("roll").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Đã tải dữ liệu roll: " + snapshot);
                if (snapshot.Exists)
                {
                    currentRollsCount = int.Parse(snapshot.Value.ToString());
                }
                else
                {
                    currentRollsCount = 0;
                }

                rollCountText.text = currentRollsCount.ToString();
            }
            else
            {
                Debug.LogError("Lỗi tải dữ liệu roll: " + task.Exception);
                currentRollsCount = 0;
            }
        });
    }

    private void CheckAndRoll()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance bị null!");
            return;
        }

        Debug.Log("Gọi PlaySFX(ButtonUI)");
        AudioManager.Instance.PlaySFX("ButtonUI");
        if (currentRollsCount > 0)
        {
            ResetPath();
            currentRollsCount--;

            rollCountText.text = currentRollsCount.ToString();

            // Cập nhật lại số roll trên Firebase
            dbReference.Child("Users").Child(userID).Child("roll").SetValueAsync(currentRollsCount);
        }
        else
        {
            Debug.Log("Không đủ lượt roll!");
        }
    }

    public void StartGame(){
        UIScreen.SetActive(true);
        //if (AudioManager.Instance == null)
        //{
        //    Debug.LogError("AudioManager.Instance bị null!");
        //    return;
        //}

        //Debug.Log("Gọi PlaySFX(ButtonUI)");
        //AudioManager.Instance.PlaySFX("ButtonUI");

        Destroy(gameObject);
    }
}
