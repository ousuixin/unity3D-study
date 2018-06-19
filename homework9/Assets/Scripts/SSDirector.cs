using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IControler
{

    public enum State { BOAT_STOP_ON_THE_LEFT_SHORE, BOAT_STOP_ON_THE_RIGHT_SHORE, BOAT_MOVING_FROM_LEFT_TO_RIGHT, BOAT_MOVING_FROM_RIGHT_TO_LEFT, WIN, LOSE };

    public interface IUserAction
    {
        void restart();
        //监听用户点击
        void clickOne();
        void prompt();
    }

    public class SSDirector : System.Object
    {
        public State state = State.BOAT_STOP_ON_THE_LEFT_SHORE;
        private static SSDirector _instance;
        private FirstSceneControl currentSceneController;

        public static SSDirector getInstance()
        {
            if (_instance == null)
            {
                _instance = new SSDirector();
            }
            return _instance;
        }

        public FirstSceneControl getCurrentSceneController()
        {
            return currentSceneController;
        }

        public void setCurrentSceneController(FirstSceneControl currentSceneController)
        {
            if (this.currentSceneController == null)
            {
                this.currentSceneController = currentSceneController;
            }
        }
    }
}
