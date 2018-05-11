using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface UserAction {
    void movePlayer(float h, float v);
    void jump();
    bool isGameOver();
    void reStart();
}
