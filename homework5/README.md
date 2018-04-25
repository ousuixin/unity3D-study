### 演示视频地址：    

[动力学方法实现](http://v.youku.com/v_show/id_XMzU2MzEyNjA5Ng==.html?spm=a2h3j.8428770.3416059.1)    
[运动学方法实现](http://v.youku.com/v_show/id_XMzU2MzEzMDMyOA==.html?spm=a2h3j.8428770.3416059.1)


### 简要说明

- 两种方法分别使用两个singleton的类PhysisActionManager和KinematicActionManager（运动学动作管理员）表示，这些类使用同一个接口IActionManager，IActionManager只有一个方法launchDisks；

  我们根据用户的选择，使用PhysisActionManager或KinematicActionManager的实例初始化FirstSceneController中的IActionManager类型变量acm；    
  
  然后在实际抛出飞盘的时候调用acm的launchDisks，这样acm会根据实例化它的那个singleton类来调用对应的launchDisks函数；
 
- 动力学方法的实现： 由于上次的飞碟游戏自己就是使用动力学方法实现的，所以，这一部分所需要的修改较少，直接将抛飞碟的动作分离出来，放到PhysisActionManager的launchDisks函数中即可；  
  
  具体做法是为飞盘加上一个瞬间的、方向大小随机的冲量（forcemode = impulse），给它一个方向大小随机的初始速度（f*0.02=m*v），然后飞盘将以此初速读在重力加速度作用下做抛体运动；  

- 运动学方法的实现： 这次主要的工作就是为飞盘加上运动学方法的实现，我们在KinematicActionManager的launchDisks函数中为飞盘加上一个ParabolicMovement（抛体运动）的脚本，这个脚本会使得飞盘做一个初速度大小方向随机的抛体运动；

  具体做法是在脚本的update函数中改变gameObject的position，这个方法在之前的第二次作业中就使用过，代码基本相同：  
    
    ```
      public void Update()
      {
          timePassed += Time.deltaTime;
          gravitySpeed.y = timePassed * gravity;
          distanceWalkedBySpeed = Speed * Time.deltaTime;
          distanceWalkedVertical = gravitySpeed * Time.deltaTime;

          transform.position += distanceWalkedBySpeed + distanceWalkedVertical;
      }
    ```
  
    这时飞碟的位置首先在原来的速度方向完成一段位移，然后在竖直方向完成一段加速度为20的位移，这样一来，飞盘就能够完成抛体运动（相当于manager抛出一个飞盘）
