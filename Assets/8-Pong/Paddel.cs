using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Paddel : MonoBehaviour
{
    static readonly int emissionColorId = Shader.PropertyToID("_EmissionColor"), timeOfLastHitId = Shader.PropertyToID("_TimeOfLastHit");
    
    [SerializeField, Min(0f)]
    float maxExtents = 4f,minExtents =4f,speed = 10f,maxTargetingBias =0.75f;
    
    [SerializeField]
    TextMeshPro scoreText;
    
    int score;
    float targetingBias;
    float extents = 4f;

    Material paddleMaterial , goalMaterial;

    [SerializeField]
    MeshRenderer goalRenderer;

    [SerializeField, ColorUsage(true, true)]
    Color goalColor = Color.white;

    public bool isAI = false;

    public void SetTargetingBias() => targetingBias = Random.Range(-maxTargetingBias, maxTargetingBias);
    
    private void Awake()
    {
        goalMaterial = goalRenderer.material;
        goalMaterial.SetColor(emissionColorId, goalColor);
        paddleMaterial = GetComponent<MeshRenderer>().material;
        SetScore(0);
    }
    void SetExtents(float newExtents)
    {
        extents = newExtents;
        Vector3 s = transform.localScale;
        s.x = 2*extents;
        transform.localScale = s;
    }

    public void Move(float target,float arenaExtents)
    {
        Vector3 position = transform.localPosition;
        position.x = isAI ? AdjustByAI(position.x, target) : AdjustByPlayer(position.x);
        float limit = arenaExtents - extents;
        position.x =  Mathf.Clamp(position.x, -limit,limit);
        transform.localPosition = position;
    }
    float AdjustByAI(float x, float target)
    {
        target += targetingBias * extents;
        if (x < target)
        {
            return Mathf.Min(x + speed * Time.deltaTime, target);
        }
        return Mathf.Max(x - speed * Time.deltaTime, target);
    }
    float AdjustByPlayer(float x)
    {
        bool goRight = Input.GetKey(KeyCode.RightArrow);
        bool goLeft = Input.GetKey(KeyCode.LeftArrow);
        if (goRight && !goLeft)
        {
            return x + speed * Time.deltaTime;
        }
        else if (goLeft && !goRight)
        {
            return x - speed * Time.deltaTime;
        }
        return x;
    }

    public bool HitBall(float ballX , float ballExtents ,out float hitFactor)
    {
        SetTargetingBias();
            hitFactor = (ballX - transform.position.x) / (ballExtents + extents);

        bool success = -1f <= hitFactor && hitFactor <= 1f;
        if (success)
        {
            paddleMaterial.SetFloat(timeOfLastHitId, Time.time);
        }
        return success;
    }
    void SetScore(int newScore, float pointsToWin =1000f)
    {
        score = newScore;
        scoreText.SetText("{0}", newScore);
        SetExtents(Mathf.Lerp(maxExtents, minExtents, newScore / (pointsToWin - 1f)));
    }

    public void StartNewGame()
    {
        SetTargetingBias();
        SetScore(0);
    }

    public bool ScorePoint(int pointsToWin)
    {
        goalMaterial.SetFloat(timeOfLastHitId, Time.time);
        SetScore(score + 1,pointsToWin);
        return score >= pointsToWin;
    }
}
