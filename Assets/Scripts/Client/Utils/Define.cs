public class Define
{
    public enum WorldObject
    {
        Unknown, MyPlayer, CommonPlayer, Monster, Object
    }

	public enum Anime
	{
		Idle, Run, Dash, Attack, Skill, Hit, Death,
	}

    public enum Layer
    {
        Object  = 7,
        Monster = 8,
        Ground  = 9,
        Block   = 10,
        Player  = 11,
    }

    public enum SceneType
    {
        Unknown, Login, Game
    }
    
    public enum SceneName
    {
        Unknown, Login, Village, Stage1, Stage2
    }

    public enum Sound
    {
        Bgm, Effect, MaxCount,
    }

    public enum UIEvent
    {
        Click, Drag,
    }

    public enum MouseEvent
    {
        Press, PointerDown, PointerUp, Click,
    }

    public enum CameraMode
    {
        QuarterView, MiniMap
    }
}
