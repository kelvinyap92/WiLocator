package cs.wmich.edu;



import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;

public class MainActivity extends Activity{
	

    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        
    }
    

   	
	public void addBlacklist(View v){
  		 Intent intent = new Intent(this, AddBlacklist.class);
  		 startActivity(intent);
  	}
	
  	public void calibrate(View v){
  		 Intent intent = new Intent(this, Calibrate.class);
  		 startActivity(intent);
  	}
	
	public void settings(View v) {
		Intent intent = new Intent(this, Settings.class);
		startActivity(intent);
	}
	
    // Call the Add activity when Add Fingerprint button is hit
   	public void add(View v){
   		// Intent intent = new Intent(this, AddFingerprint.class);
   		 Intent intent = new Intent(this, AddFingerprint.class);
   		 startActivity(intent);
   	}
   	
   	public void locate(View v) {
		Intent intent = new Intent(this, Locate.class);
		startActivity(intent);
	}

   	
   	//To be delete
   	public void temp(View v) {
		Intent intent = new Intent(this, Temp.class);
		startActivity(intent);
	}


}
