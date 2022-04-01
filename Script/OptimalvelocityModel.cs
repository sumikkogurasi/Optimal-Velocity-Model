using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Optimal velocity Model
/// </summary>
/// 

public class OptimalvelocityModel : MonoBehaviour
{
    //N個の粒子数
    [SerializeField]
    private static int numCar = 100;

    [Header("粒子上限数")]
    public GameObject[] allCar = new GameObject[numCar];

    [Header("インスタンス生成")]
    public GameObject boxPrefab;

    [Header("粒子数")]
    public int ActuallyNumCar;

    [Header("半径")]
    [SerializeField]
    float radius = 2f;

    [Header("感応度")]
    [SerializeField]
    double sensitivity = 1.0d;

    [Header("誤差範囲")]
    [SerializeField]
    float section = 0.1f;

    [Header("描画頻度")]
    public int Scale = 1;

    [Header("カメラ距離")]
    public Camera m_OrthographicCamera;
    public int CameraSize = 20;

    //データ保管用配列群　（距離）

    //１フレーム前の距離
    double[] PreDis = new double[numCar];

    //現在の距離
    double[] NowDis = new double[numCar];

    //１フレーム後の距離
    double[] NextDis = new double[numCar];

    //判定素子
    private bool StopState = false;

    //実際の速度
    double[] V_r = new double[numCar];

    //加速度
    double[] a = new double[numCar];

    //δタイム
    double dt = 0.01;

    void Start()
    {

    }

    private void Update()
    {
        if (StopState)
        {
            //内部処理
            Real();

            //描画
            draw();

        }

        //描画を間引く間隔
        TimeScale();

        //カメラ距離
        CameraScale();

    }

    //描画
    void draw()
    {
        double angle = 0;

        for (int i = 0; i < ActuallyNumCar; i++)
        {
            angle = Mathf.Rad2Deg * (NowDis[i] / radius);

            allCar[i].transform.rotation = Quaternion.Euler(0, 0, (float)angle);
        }
    }

    //内部処理
    void Real()
    {
        for (int i = 0; i < Scale; i++)
        {
            for (int j = 0; j < ActuallyNumCar; j++)
            {
                //速度決定
                RealVelocity(j);

            }
            for (int j = 0; j < ActuallyNumCar; j++)
            {
                //渋滞モデル
                Traffic(j);

            }
            for (int j = 0; j < ActuallyNumCar; j++)
            {
                //位置決定
                Realposition(j);

            }
            for (int j = 0; j < ActuallyNumCar; j++)
            {
                //データ遷移
                NextFrame(j);

            }
        }
    }

    //速度決定
    void RealVelocity(int j)
    {
        //実際の速度
        V_r[j] = (NowDis[j] - PreDis[j]) / dt;
    }

    //渋滞モデル
    void Traffic(int j)
    {
        //こうありたいと考える速度の位置
        double V_w = 0;

        //こうありたいと考える速度の位置
        //V(x) = tanh(x-2) + tanh(2) V(x_(i+1)-x_i) = tanh((x_(i+1)-x_i)-2) + tanh(2)
        if (j < ActuallyNumCar - 1)
        {
            double x = NowDis[j + 1] - NowDis[j];

            V_w = System.Math.Tanh(x - 2) + System.Math.Tanh(2);
        }
        else if (j == ActuallyNumCar - 1)
        {
            double x = 2 * Mathf.PI * radius + NowDis[0] - NowDis[ActuallyNumCar - 1];

            V_w = System.Math.Tanh(x - 2) + System.Math.Tanh(2);
        }

        a[j] = sensitivity * (V_w - V_r[j]);

        //Debug.Log("V_w:" + V_w);
        //Debug.Log("V_r:" + V_r[j]);
        //Debug.Log("V_a:" + V_a[j]);
    }


    //位置決定
    void Realposition(int j)
    {
        NextDis[j] = (2 * NowDis[j] - PreDis[j]) + a[j] * dt * dt;
    }

    //データ遷移
    void NextFrame(int j)
    {
        PreDis[j] = NowDis[j];

        NowDis[j] = NextDis[j];

        //Debug.Log("NowDis[j]:" + NowDis[j]);
    }

    //すべての粒子を削除する
    void DeleteObject()
    {
        for (int i = 0; i < numCar; i++)
        {
            Destroy(allCar[i]);
        }
        Destroy(ParentObject);
    }

    //親オブジェクト
    private GameObject ParentObject;

    //N個の粒子数の生成とそれぞれの初期位置を設定する
    void CreateObject()
    {
        //親を生成
        GameObject parentTran = new GameObject("Cars");

        ParentObject = parentTran;

        //粒子数に応じて角度を設定（ラジアン）
        float radian = Mathf.PI * 2 / ActuallyNumCar;

        for (int i = 0; i < ActuallyNumCar; i++)
        {
            GameObject Shaft = new GameObject("Shaft" + i);

            Shaft.transform.parent = parentTran.transform;
            Vector3 pos = new Vector3(radius, 0, 0);
            Instantiate(boxPrefab, Shaft.transform.position + pos, Quaternion.identity, Shaft.transform);

            float error = Random.Range(0, section);
            Shaft.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * (radian * i + error));

            allCar[i] = Shaft;
        }

    }

    //初期位置、初期角度を取得
    void StartPosition()
    {
        for (int i = 0; i < ActuallyNumCar; i++)
        {
            if (allCar[i].transform.eulerAngles.z < 0)
            {
                //x = rθ
                PreDis[i] = radius * (2 * Mathf.PI + Mathf.Deg2Rad * (allCar[i].transform.eulerAngles.z));

                NowDis[i] = radius * (2 * Mathf.PI + Mathf.Deg2Rad * (allCar[i].transform.eulerAngles.z));

            }
            else
            {
                PreDis[i] = radius * Mathf.Deg2Rad * allCar[i].transform.eulerAngles.z;

                NowDis[i] = radius * Mathf.Deg2Rad * allCar[i].transform.eulerAngles.z;

            }
        }
    }

    //操作パネル
    [Header("操作パネル")]
    public InputField inputFieldNum;
    public InputField inputFieldRadius;
    public InputField inputFieldSens;
    public InputField inputFieldRandom;

    public Text Numtext;
    public Text Radiustext;
    public Text Senstext;
    public Text Randomtext;
    public Text Scaletext;
    public Text CamSizetext;

    //ジェネレートボタン
    public void Method()
    {
        //入力数値代入
        SetValue();

        //粒子削除
        DeleteObject();

        //粒子生成
        CreateObject();

        //粒子初期位置
        StartPosition();

        StopState = true;
    }

    //入力数値代入
    void SetValue()
    {
        ActuallyNumCar = int.Parse(Numtext.text);

        radius = float.Parse(Radiustext.text);

        sensitivity = double.Parse(Senstext.text);

        section = float.Parse(Randomtext.text);

        Debug.Log("ActuallyNumCar:" + ActuallyNumCar);
        Debug.Log("radius:" + radius);
        Debug.Log("sensitivity:" + sensitivity);
        Debug.Log("section:" + section);
    }

    public void inputNum()
    {
        //テキストにinputFieldの内容を反映
        Numtext.text = inputFieldNum.text;
    }

    public void inputRadius()
    {
        //テキストにinputFieldの内容を反映
        Radiustext.text = inputFieldRadius.text;
    }

    public void inputSens()
    {
        //テキストにinputFieldの内容を反映
        Senstext.text = inputFieldSens.text;
    }
    public void inputRandom()
    {
        //テキストにinputFieldの内容を反映
        Randomtext.text = inputFieldRandom.text;
    }

    //倍速
    [Header("倍速")]
    [SerializeField]
    private Slider ScaleSlider;

    //描画の間引き間隔
    public void TimeScale()
    {
        Scale = (int)ScaleSlider.value;
        Scaletext.text = Scale.ToString();
    }

    //カメラ距離
    [Header("カメラ距離")]
    [SerializeField]
    private Slider CamSlider;

    //カメラ距離
    public void CameraScale()
    {
        CameraSize = (int)CamSlider.value;
        CamSizetext.text = CameraSize.ToString();

        m_OrthographicCamera.orthographicSize = CameraSize;
    }
}
