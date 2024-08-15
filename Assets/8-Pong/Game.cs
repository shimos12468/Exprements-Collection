using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{

    public Ball ball;
    public Paddel topPaddel, bottomPaddel;


    [SerializeField, Min(0f)]
    Vector2 arenaExtents = new Vector2(10, 10);

    [SerializeField]
    TextMeshPro countdownText;

    [SerializeField, Min(1f)]
    float newGameDelay = 3f;

    float countdownUntilNewGame;


    [SerializeField, Min(2)]
    int pointsToWin = 3;


    [SerializeField]
    LivelyCamera livelyCamera;
    void Awake() => countdownUntilNewGame = newGameDelay;

    

    // Update is called once per frame
    void Update()
    {
        bottomPaddel.Move(ball.Position.x, arenaExtents.x);
        topPaddel.Move(ball.Position.x,arenaExtents.x);

        if (countdownUntilNewGame <= 0f)
        {
            countdownText.gameObject.SetActive(false);
            UpdateGame();
        }
        else
        {
            UpdateCountDown();
        }

       
    }

    private void UpdateCountDown()
    {
       countdownUntilNewGame-=Time.deltaTime;
        
       if (countdownUntilNewGame <= 0f)
        {
            countdownText.gameObject.SetActive(false);
            StartNewGame();
        }
        else
        {
            float displayValue = Mathf.Ceil(countdownUntilNewGame);
            if (displayValue< newGameDelay)
            {
                countdownText.SetText("{0}", displayValue);
            }
        }
    }

    private void StartNewGame()
    {
        ball.StartNewGame();
        bottomPaddel.StartNewGame();
        topPaddel.StartNewGame();
    }

    private void UpdateGame()
    {
        ball.Move();
        BounceYIfNeeded();
        BounceXIfNeeded(ball.Position.x);
        ball.UpdateVisualization();
    }

    private void BounceYIfNeeded()
    {
       float yExtents = arenaExtents.y-ball.Extents;

        if (ball.Position.y < -yExtents)
        {
            BounceY(-yExtents,bottomPaddel,topPaddel);
        }
        else if(ball.Position.y > yExtents)
        {
            BounceY(yExtents,topPaddel, bottomPaddel);
        } 
    }

    private void BounceY(float boundary, Paddel defender , Paddel attacker)
    {
        float durationAfterBounce = (ball.Position.y - boundary) / ball.Velocity.y;
        float bounceX = ball.Position.x - (ball.Velocity.x*durationAfterBounce);
        BounceXIfNeeded(bounceX);
        bounceX = ball.Position.x - ball.Velocity.x * durationAfterBounce;
        livelyCamera.PushXZ(ball.Velocity);
        ball.BounceY(boundary);

        if(defender.HitBall(bounceX,ball.Extents,out float hitFactor))
        {
            livelyCamera.JostileY();
            ball.SetXPositionAndSpeed(bounceX, hitFactor, durationAfterBounce);
        }
        else if (attacker.ScorePoint(pointsToWin))
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        countdownUntilNewGame = newGameDelay;
        countdownText.SetText("GAMEOVER");
        countdownText.gameObject.SetActive(true);
        ball.EndGame();
    }

    private void BounceXIfNeeded(float x)
    {
        float xExtents = arenaExtents.x - ball.Extents;

        if (x < -xExtents)
        {
            livelyCamera.PushXZ(ball.Velocity);
            ball.BounceX(-xExtents);
        }
        else if (x > xExtents)
        {
            livelyCamera.PushXZ(ball.Velocity);
            ball.BounceX(xExtents);
        }
    }
}
