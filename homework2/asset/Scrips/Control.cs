using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IControler;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace IControler
{

    public enum State { BOAT_STOP_ON_THE_LEFT_SHORE, BOAT_STOP_ON_THE_RIGHT_SHORE, BOAT_MOVING_FROM_LEFT_TO_RIGHT, BOAT_MOVING_FROM_RIGHT_TO_LEFT, WIN, LOSE };

    public interface ISceneController
    {
        void loadResources();
    }

    public interface IUserAction
    {
        //牧师在左边/右边河岸上/下船；
        void priestLeftshoreGoOnboard();
        void priestRightshoreGoOnboard();
        //魔鬼在左边/右边河岸上/下船；
        void devilLeftshoreGoOnboard();
        void devilRightshoreGoOnboard();
        //牧师/魔鬼在左边/右边河岸上/下船
        void LeftshoreDisembark();
        void RightshoreDisembark();
        //开船
        void moveBoat();
        //重新开始游戏
        void restart();
    }

    public class SSDirector : System.Object, IUserAction
    {
        private static SSDirector _instance;
        public Control _baseCode;
        private Model gameObjectModel;
        public State state = State.BOAT_STOP_ON_THE_LEFT_SHORE;

        public static SSDirector getInstance()
        {
            if (_instance == null)
            {
                _instance = new SSDirector();
            }
            return _instance;
        }

        public Control getBaseCode()
        {
            return _baseCode;
        }

        public void setBaseCode(Control baseCode)
        {
            if (_baseCode == null)
            {
                _baseCode = baseCode;
            }
        }

        public Model getGenGameObject()
        {
            return gameObjectModel;
        }

        internal void setGenGameObject(Model gom)
        {
            if (null == gameObjectModel)
            {
                gameObjectModel = gom;
            }
        }

        public void priestLeftshoreGoOnboard()
        {
            gameObjectModel.leftShorePriestGoOnBoat();
        }

        public void priestRightshoreGoOnboard()
        {
            gameObjectModel.rightShorePriestGoOnBoat();
        }

        public void devilLeftshoreGoOnboard()
        {
            gameObjectModel.leftShoreDevilGoOnBoat();
        }

        public void devilRightshoreGoOnboard()
        {
            gameObjectModel.rightShoreDevilGoOnBoat();
        }

        public void LeftshoreDisembark()
        {
            gameObjectModel.GoOffBoard(0);
        }

        public void RightshoreDisembark()
        {
            gameObjectModel.GoOffBoard(1);
        }

        public void moveBoat()
        {
            gameObjectModel.moveBoat();
        }
        
        public void restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            state = State.BOAT_STOP_ON_THE_LEFT_SHORE;
        }
    }
}

public class Control : MonoBehaviour
{
    public string gameName = "牧师与魔鬼";
    public string gameRules = "你要运用智慧帮助3个" +
        "牧师（方块）和3个魔鬼（圆球）渡河。船最多可以载2名游" +
        "戏角色。船上有游戏角色时，你才可以点击这个船，让船移动到" +
        "对岸。当有一侧岸的魔鬼数多余牧师数时（包括船上的魔鬼和牧师" +
        "），魔鬼就会失去控制，吃掉牧师（如果这一侧没有牧师则不会失" +
        "败），游戏失败。当所有游戏角色都上到对岸时，游戏胜利。";

    private void Start()
    {
        SSDirector gameDirector = SSDirector.getInstance();
        gameDirector.setBaseCode(this);
    }
}
