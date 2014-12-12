package cs.wmich.edu;



import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;

/**
 * Called the activity when it is created. This class contains the main page of the navigation to each menu
 * @author kelvinyap
 *
 */
public class MainActivity extends Activity{
	

    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        
    }
    
    /**
     * Call Blacklist MAC Address activity
     */
	public void addBlacklist(View v){
  		 Intent intent = new Intent(this, AddBlacklist.class);
  		 startActivity(intent);
  	}
	/**
     * called Calibrate  activity
     */
	     public void calibrate(View v){
  		 Intent intent = new Intent(this, Calibrate.class);
  		 startActivity(intent);
  	}
	
	     /**
	      * called Setting  activity
	      */
	public void settings(View v) {
		Intent intent = new Intent(this, Settings.class);
		startActivity(intent);
	}
	
	/**
     * called Setup Fingerprint  activity
     */
	   public void setupFingerprint(View v){
   		 Intent intent = new Intent(this, SetupFingerprint.class);
   		 startActivity(intent);
   	}
   	
	   /**
	     * called Locate  activity
	     */
	   public void broadCastEmergency(View v) {
		Intent intent = new Intent(this, Broadcast_Emergency.class);
		startActivity(intent);
	}

   	
   	//This was to show the view to test the auto locate function
   	/**
   	public void temp(View v) {
		Intent intent = new Intent(this, Temp.class);
		startActivity(intent);
	}
	**/


}
