using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundInput;

public class VoiceRing_Script : MonoBehaviour {

	/*public float LineWidth =2f;
	//public VectorLine graph;
    public List<Vector3> graphCoords;
    public Vector2[] calcPos = new Vector2[128];
    public Texture _lineMaterial;
    float[] lpcMirror;*/
	public SoundInputController SIC;
	public GameObject particlePrefab;
	[Range(10, 500)]
	public int amountOfParticles;
	public float particleMoveSpeed;

	GameObject[] particles;
	GameObject particle;

	public Color[] pitchColors;



	float pitch = 0.0f;
	float currentPitch = 0.0f;
	float volume = 0.0f;
	float currentVolume = 0.0f;
	

	float SineFunction (float x, float t) {
		float y = Mathf.Sin(Mathf.PI *(pitch*0.25f) * (x + t));
		y *= 1f / 5f;
		return y;
	}

	float MultiSineFunction (float x, float t) {
		float y = Mathf.Sin(Mathf.PI *(pitch+0.1f) * (x + t));
		y += Mathf.Sin(2f * Mathf.PI*(pitch+0.1f) * (x + 2f * t)) / 2f;
		y *= 1f / 10f;
		return y;
	}

	private void Start()
	{
		//pSys = this.GetComponent<ParticleSystem>();
		//print(Mathf.Sin(256/256*2f*Mathf.PI));

		particles = new GameObject[amountOfParticles];

		if(particlePrefab != null){

			for (int i = 0; i < particles.Length; i++)
			{
				particle = Instantiate(particlePrefab) as GameObject;
				particle.transform.parent = this.transform;
				particles[i] = particle;
			}

		}
	}
	
	void Update()
    {
		
		
		currentPitch = SIC.inputData.relativeFrequency;
		currentVolume = SIC.inputData.relativeAmplitude;

        pitch = Mathf.Lerp(pitch, currentPitch, Time.deltaTime*2f);
		if(currentVolume > 0){
			volume = Mathf.Lerp(volume, currentVolume, Time.deltaTime/2f);
		} else {
			volume = 1.0f;
		}

		float t = Time.time;
		
		
		Color partColor = Color.white;

		/*for(int j = 0; j < pitchColors.Length; j++){

			if(pitch > (float)(j/pitchColors.Length) && pitch <= (float)(j+1/pitchColors.Length)){
				partColor = pitchColors[j];
				
			}
			
		}*/
		
		partColor = Color.Lerp(pitchColors[0], pitchColors[1], pitch);
		
		
		//realtime particles
        for (int i = 0; i < particles.Length; i++)
        {
			//set position for each particle
			Vector3 position = this.transform.localPosition;
			Vector2 offset = new Vector2(0,0);

			offset = new Vector2(Mathf.Sin(i/(float)(particles.Length)*2f*Mathf.PI), Mathf.Cos(i/(float)(particles.Length)*2f*Mathf.PI))*(volume/2f);

			position.x += offset.x;

			position.z += offset.y;

			position.y = SineFunction(i, t);

			particles[i].transform.localPosition = position;

			ParticleSystem.VelocityOverLifetimeModule veloMain = particles[i].GetComponent<ParticleSystem>().velocityOverLifetime;

			Vector3 velocity = Vector3.Normalize(position - this.transform.localPosition)*particleMoveSpeed;

			veloMain.x = velocity.x;
			veloMain.z = velocity.z;

			ParticleSystem.EmissionModule emis = particles[i].GetComponent<ParticleSystem>().emission;

			if(currentVolume > 0){
				emis.rateOverTime = 5;
			} else {
				emis.rateOverTime = 0;
			}

			ParticleSystem.MainModule main = particles[i].GetComponent<ParticleSystem>().main;

			main.startColor = partColor;

			particles[i].GetComponentInChildren<MeshRenderer>().material.SetColor("_TintColor", partColor);

		}
		
    }
}
