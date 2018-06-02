using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyParticleSystem : MonoBehaviour {
    private ParticleSystem system;
    private ParticleSystem.Particle[] particles;
    private ParticlePosition[] particlePositions;

    public int count = 6000;               // 粒子数量  
    public float size = 0.02f;              // 粒子大小  
    public float speed = 1f;                // 粒子旋转速度
    public float pingPong = 0.02f;          // 粒子游离范围

    public float minRadius = 2.0f;          // 粒子环最小半径  
    public float maxRadius = 3.5f;          // 粒子环最大半径   

    private int tier = 10;                  // 速度差分层数  
    private Gradient colorGradient = new Gradient();

    // Use this for initialization
    void Start()
    {
        // 初始化粒子数组  
        particles = new ParticleSystem.Particle[count];
        particlePositions = new ParticlePosition[count];

        // 初始化粒子系统  
        system = this.GetComponent<ParticleSystem>();
        var temp = system.main;
        temp.startSpeed = 0;                // 粒子位置由程序控制  
        temp.startSize = size;              // 设置粒子大小  
        temp.loop = false;
        temp.maxParticles = count;          // 设置最大粒子量  
        system.Emit(count);                 // 发射粒子  
        system.GetParticles(particles);

        // 初始化梯度颜色控制器  
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[6];
        alphaKeys[0].time = 0.0f; alphaKeys[0].alpha = 0.5f;
        alphaKeys[1].time = 0.125f; alphaKeys[1].alpha = 0.0f;
        alphaKeys[2].time = 0.375f; alphaKeys[2].alpha = 1.0f;
        alphaKeys[3].time = 0.625f; alphaKeys[3].alpha = 0.0f;
        alphaKeys[4].time = 0.875f; alphaKeys[4].alpha = 1.0f;
        alphaKeys[5].time = 1.0f; alphaKeys[5].alpha = 0.5f;
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].time = 0.0f; colorKeys[0].color = Color.white;
        colorKeys[1].time = 1.0f; colorKeys[1].color = Color.white;
        colorGradient.SetKeys(colorKeys, alphaKeys);

        RandomlySpread();                   // 初始化各粒子位置  
    }


    void RandomlySpread()
    {
        for (int i = 0; i < count; ++i)
        {   
            // 随机每个粒子距离中心的半径，同时希望粒子集中在平均半径附近
            float midRadius = (maxRadius + minRadius) / 2;
            float minRadiusOfOneParticle = Random.Range(minRadius, midRadius);
            float maxRadiusOfOneParticle = Random.Range(midRadius, maxRadius);
            float radius = Random.Range(minRadiusOfOneParticle, maxRadiusOfOneParticle);

            // 随机每个粒子的角度    
            float angle = Random.Range(0.0f, 360.0f);
            float theta = angle / 180 * Mathf.PI;

            // 随机每个粒子的游离起始时间    
            float time = Random.Range(0.0f, 1000f);

            // 设置粒子顺时针/逆时针
            if (Random.Range(0f, 2f) > 1f)
            {
                particlePositions[i] = new ParticlePosition(radius, angle, time, true);
            }
            else
            {
                particlePositions[i] = new ParticlePosition(radius, angle, time, false);
            }

            // 使用上述参数构造粒子
            particles[i].position = new Vector3(particlePositions[i].radius * Mathf.Cos(theta), 0f, particlePositions[i].radius * Mathf.Sin(theta));

        }

        system.SetParticles(particles, particles.Length);
    }

    void Update()
    {
        for (int i = 0; i < count; i++)
        {

            if (particlePositions[i].clockDirection)             // 顺时针旋转  
            {
                particlePositions[i].angle -= ( i % tier + 1 ) * ( speed / particlePositions[i].radius / tier );
            }
            else                            // 逆时针旋转 
            {

                particlePositions[i].angle += ( i % tier + 1 ) * ( speed / particlePositions[i].radius / tier );
            }

            particlePositions[i].time += Time.deltaTime;
            particlePositions[i].radius += Mathf.PingPong(particlePositions[i].time / minRadius / maxRadius, pingPong) - pingPong / 2.0f;

            float theta = ( particlePositions[i].angle % 360 ) / 180 * Mathf.PI;
            while ( theta > 2 * Mathf.PI)
            {
                theta = theta - 2 * Mathf.PI;
            }

            particles[i].position = new Vector3(particlePositions[i].radius * Mathf.Cos(theta), 0f, particlePositions[i].radius * Mathf.Sin(theta));


            particles[i].startColor = colorGradient.Evaluate( theta / 2 / Mathf.PI);
        }

        // 调整透明度
        system.SetParticles(particles, particles.Length);
    }
}
