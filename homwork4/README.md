[游戏视频链接](http://v.youku.com/v_show/id_XMzU0NDU3MTY2NA==.html?spm=a2hzp.8244740.0.0)

## 游戏概况：
- 游戏有n个round，每个round都包括10次trial（即有十次发射飞盘的机会）；
- 玩家按下空格键发射飞盘，飞碟的色彩、发射位置、角度、同时出现的个数都不同；
- 第n个round时，每次按下空格都会发射处n个飞盘，相邻两个飞盘发射的时间随着round增加二减少（难度逐渐增加）；
- 鼠标点击发射子弹，子弹打中飞盘就得10分；  

## 学习心得：
- 这次学习了一下力的添加（addforce函数），简单的说一下这个函数的四种模式
  - 计算公式：    Ft = mv(t) 即 v(t) = Ft/m， 注意在施加力addforce的时候是认为该刚体只受到这一个力（不受其他力），而且这些力调用一次只在调用时的那一秒有效，如果想要持续施加力，需要在update中调用

  - (1)ForceMode.Force : 持续施加一个力，与重力mass有关，t = 每帧间隔时间（默认0.02s），m = mass，也就是说在调用该函数的那一秒中的前0.02s，物体受到大小F牛的合力，之后的0.98s该力消失（重力等力恢复）

  - (2)ForceMode.Impulse : 瞬间施加一个力，与重力mass有关，t = 1.0f，m = mass，也就是说在调用该函数的那一秒中，物体一直受到大小F牛的合力，1s之后，如果不再调用一次该函数，则该力消失（重力等力恢复）

  - (3)ForceMode.Acceleration：持续施加一个力，与重力mass无关，t = 每帧间隔时间（默认0.02s），m = 1.0f，具体效果类似（1）

  - (4)ForceMode.VelocityChange：瞬间施加一个力，与重力mass无关，t = 1.0f，m = 1.0f，具体效果类似（2）
