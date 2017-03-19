using UnityEngine;
using System.Collections;

public abstract class GeneralBehaviour : MonoBehaviour, IDamagable{
	protected Light Light;
	protected float timer=0;
	protected int damage;
	protected float delay;
	protected int mag;
	protected int magFull;
	protected float reload;
	protected float spread;
	protected int velocity=1500;
	protected float health;
	protected float healthMax;
	protected bool isFriendly;

	protected Vector3 vec=new Vector3(0f,0.5f,0f);
	protected Vector3 vec2=new Vector3(0f,1f,0f);
	protected bool tex=false;

	protected string targetName;
	protected string friendlyName;
	protected GameObject Target;

	public MainDoc Control;
	public MapGen Map;
	public CoverControl CoverCont;
	public GameObject Projectile;
	public GameObject Flash;
	
	protected virtual void Awake(){
		Control=GameObject.FindWithTag("GameController").GetComponent<MainDoc>();//get reference to GameController
		Map = GameObject.FindWithTag("GameController").GetComponent<MapGen>();
		CoverCont = GameObject.FindWithTag("GameController").GetComponent<CoverControl>();//get reference to CoverController
		Light = gameObject.transform.GetChild(0).GetComponent<Light>();//get reference to object's light
	}

	public virtual void SetStats(bool isFriend, int am,float del,int dmg,float hp,float rel,float acc, string t,string f){
		isFriendly = isFriend;
		magFull = am;
		mag = am;
		delay = del;
		damage = dmg;
		health = hp;
		healthMax = hp;
		reload=rel;
		spread = acc;
		targetName = t;
		friendlyName = f;
	}

	protected void Shoot(){
		if (timer > delay){
			if(Target==null||mag%Mathf.Floor(magFull*0.25f)==0)//find target if it is not set and also every time around a quarter of the mag is spent
				Target=Control.GetTarget(isFriendly, gameObject);
			if (Target == null)
				 return;
			GameObject temp;
			Quaternion tem=Quaternion.LookRotation((Target.transform.position - this.transform.position));//rotate to face target
			Instantiate(Flash,this.transform.position+vec,tem);//create a muzzle flash
			tem*=Quaternion.AngleAxis((spread/2)*(Random.value-0.5f),Vector3.up);//add spread
			tem*=Quaternion.AngleAxis(spread*(Random.value-0.5f),Vector3.left);
			temp=Instantiate(Projectile, this.transform.position+vec2, tem)as GameObject;//instantiate and save a bullet
			temp.GetComponent<ProjectileBehaviour>().SetStats(targetName,friendlyName,damage);//set bullet stats
			temp.GetComponent<Rigidbody>().AddForce(temp.transform.rotation*Vector3.forward*velocity);//launch bullet
			timer=(Random.value-0.5f)*delay;//delay the next shoot
			mag--;
			if(mag<1){//reload and clear target
				Target=null;
				gameObject.GetComponent<AudioSource>().Play();
				mag=magFull;
				timer=reload*(Random.value-1.5f);
			}
		}
	}
	public abstract void hit(int dmg);

	protected void adjustLight()
	{
		Light.intensity = 1f + 1.6f * (health / healthMax);
	}

	protected void RecoverHP(){
		if (health < healthMax){
			health += Time.deltaTime * healthMax * 0.04f;
			health = Mathf.Min (health, healthMax);
			adjustLight();
		}
	}

	public int GetRoom()
	{
		return Map.getRoom(transform.position);
	}
}
