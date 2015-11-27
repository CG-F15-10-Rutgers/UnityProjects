using UnityEngine;
using System.Collections;
using TreeSharpPlus;

public class MyBehaviorTree : MonoBehaviour
{
	public Transform meetingPoint1;
	public Transform meetingPoint2;
	public Transform chairTrigger;
	public Transform gunTrigger;
	public Transform shootPosition;
	public Transform slashPosition;
	public Transform hidePosition;
	public GameObject samurai;
	public GameObject shooter;
	public GameObject gun;
	public GameObject chair;
	public GameObject katana;

	private BehaviorAgent behaviorAgent;
	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	protected Node ST_ApproachAndWait(GameObject participant, Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(500));
	}



	protected Node ST_Sitdown() {

		Val<Vector3> ornt = Val.V (() => ((2 * samurai.transform.position)-chair.transform.position));
		return new Sequence(samurai.GetComponent<BehaviorMecanim>().Node_OrientTowards(ornt), samurai.GetComponent<BehaviorMecanim>().Node_SitDown(), new LeafWait(1000));
	}
	
	protected Node AttachGun () {
		
		return new LeafInvoke(
			() => this.gun.GetComponent<GunController>().SetIsHolding(true));
	}

	protected Node ST_HoldGun () {
		return new Sequence(shooter.GetComponent<BehaviorMecanim> ().Node_HandAnimation ("PISTOLAIM", true), new LeafWait(500), this.AttachGun());
	}

	protected Node AttachKatana () {
		
		return new LeafInvoke(
			() => this.katana.GetComponent<KatanaController>().holdKatana());
	}

	protected Node ST_DrawKatana () {
		return new Sequence(samurai.GetComponent<BehaviorMecanim> ().Node_HandAnimation ("KATANAGRAB", true), new LeafWait(3000),
		                    this.AttachKatana(), new LeafWait(500),
		                    samurai.GetComponent<BehaviorMecanim> ().Node_HandAnimation ("KATANADRAW", true), new LeafWait(1000));
	}

	protected Node ST_KatanaFight () {

		return samurai.GetComponent<BehaviorMecanim> ().Node_HandAnimation ("KATANAFIGHT", true);
	}

	protected Node GetInPosition() {

		return new SequenceParallel (
			this.ST_ApproachAndWait(shooter, shootPosition),
			new Sequence(
				new LeafWait(500), samurai.GetComponent<BehaviorMecanim>().Node_HeadLook(gun.transform.position),
				new LeafWait(1500), samurai.GetComponent<BehaviorMecanim>().Node_HeadLookStop(),
				samurai.GetComponent<BehaviorMecanim>().Node_StandUp(), new LeafWait(500),
				this.ST_ApproachAndWait(samurai, hidePosition), samurai.GetComponent<BehaviorMecanim>().Node_OrientTowards(shootPosition.position),
				this.ST_DrawKatana(), new LeafWait(9500), this.ST_ApproachAndWait(samurai, slashPosition)
			)
		);
	}

	protected Node GunFalls(){

		return new Sequence( new LeafInvoke(
			() => this.gun.GetComponent<GunController>().SetIsHolding(false)),
		    new LeafInvoke(() => this.gun.GetComponent<GunController>().SetGravity(true)));
	}

	protected Node ST_ShooterDead(){

		return new Sequence (
			this.GunFalls(), shooter.GetComponent<BehaviorMecanim> ().Node_HandAnimation ("PISTOLAIM", false), shooter.GetComponent<BehaviorMecanim>().Node_BodyAnimation("DYING", true));
	}

	protected Node ST_FightAndDie() {
		return new Sequence (
			this.ST_HoldGun(),
			this.GetInPosition(),
			this.ST_KatanaFight(), new LeafWait(1000),
			this.ST_ShooterDead()
			);
	}

	protected Node ST_Greetings() {
		return new Sequence (
			samurai.GetComponent<BehaviorMecanim>().Node_OrientTowards(meetingPoint1.transform.position),
			samurai.GetComponent<BehaviorMecanim> ().Node_HandAnimation ("WAVE", true),
			shooter.GetComponent<BehaviorMecanim>().Node_OrientTowards(meetingPoint2.transform.position),
			shooter.GetComponent<BehaviorMecanim> ().Node_HandAnimation ("WAVE", true)
			);
	}

	protected Node ST_TalkAndListen() {

		return new Sequence (
			new Sequence(new LeafWait(1000), shooter.GetComponent<BehaviorMecanim> ().Node_FaceAnimation("HEADSHAKE", true),
		             new LeafWait(1000), samurai.GetComponent<BehaviorMecanim> ().Node_FaceAnimation("HEADSHAKETHINK", true)),
			new LeafWait(1000),
			new Sequence(new LeafWait(1000), shooter.GetComponent<BehaviorMecanim> ().Node_FaceAnimation("HEADSHAKE", false),
		             new LeafWait(1000), samurai.GetComponent<BehaviorMecanim> ().Node_FaceAnimation("HEADSHAKETHINK", false))
			);
	}

	protected Node ST_Disagree(){

		return new Sequence(shooter.GetComponent<BehaviorMecanim> ().Node_HandAnimation("POINTING", true), new LeafWait(1000), shooter.GetComponent<BehaviorMecanim> ().Node_HandAnimation("POINTING", false),
		                    shooter.GetComponent<BehaviorMecanim> ().Node_HandAnimation("CUTTHROAT", true),new LeafWait(1000), shooter.GetComponent<BehaviorMecanim> ().Node_HandAnimation("CUTTHROAT", false),
		             		
							new Sequence(samurai.GetComponent<BehaviorMecanim> ().Node_HandAnimation("CLAP", true), new LeafWait(1000), samurai.GetComponent<BehaviorMecanim> ().Node_HandAnimation("CLAP", false)),
							new Sequence(samurai.GetComponent<BehaviorMecanim> ().Node_FaceAnimation("SPEW", true), new LeafWait(1000), samurai.GetComponent<BehaviorMecanim> ().Node_FaceAnimation("SPEW", true)),
		                    samurai.GetComponent<BehaviorMecanim> ().Node_FaceAnimation("ROAR", true), new LeafWait(1500), samurai.GetComponent<BehaviorMecanim> ().Node_FaceAnimation("ROAR", false)
		             );
	}

	protected Node ST_Leave() {

		return new SequenceParallel (new Sequence(this.ST_ApproachAndWait(samurai, chairTrigger), this.ST_Sitdown()), this.ST_ApproachAndWait(shooter, gunTrigger));
	}

	protected Node ST_Conversation(){
		return new Sequence (
			this.ST_Greetings(),
			this.ST_TalkAndListen(),
			this.ST_Disagree(),
			this.ST_Leave()
			);
	}

	protected Node ST_GoToMeetingPoint(){

		return new SequenceParallel (this.ST_ApproachAndWait(samurai, meetingPoint2), this.ST_ApproachAndWait(shooter, meetingPoint1));
	}

	protected Node ST_ApproachAndConversation(){
		return new Sequence (this.ST_GoToMeetingPoint (), this.ST_Conversation ());

	}

	protected Node BuildTreeRoot()
	{
		return
			new DecoratorLoop(
				new Sequence(
					this.ST_ApproachAndConversation(),
					this.ST_FightAndDie()
					//this.ST_HoldGun(),
					//this.ST_ApproachAndWait(this.wander1),
					//this.ST_ApproachAndWait(this.wander2),
					//this.ST_ApproachAndWait(this.wander3),
					//this.ST_Sitdown()
					//this.ST_DrawKatana(),
				//this.ST_KatanaFight()
				));
	}
}
