package cs.wmich.edu.wifi;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileWriter;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import cs.wmich.edu.AddFingerprint;
import cs.wmich.edu.Calibrate;
import cs.wmich.edu.Locate;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.net.wifi.ScanResult;
import android.util.Log;
import android.widget.Toast;

public class LocateReceiver extends BroadcastReceiver {
	private static final String TAG = "LocateReceiver";
	
     Locate locate;

	public LocateReceiver(Locate locate){
		super();
		this.locate = locate;
		
	}

	@Override
	public void onReceive(Context c, Intent intent) {

		List<ScanResult> results = locate.WiFi.getScanResults();
	
		


		ArrayList<WifiInfo> data = new ArrayList<WifiInfo>(results.size());
		for (ScanResult result : results.subList(0, 10)) {
			
			data.add(new WifiInfo(result.level, result.BSSID, result.SSID));
		}

		String message = String.format("%s networks found. Updating list.",
				results.size());
		//Toast.makeText(locate, message, Toast.LENGTH_SHORT).show();
		Log.d(TAG, "onReceive() message: " + message);

		StringBuilder sbs = new StringBuilder();
		StringBuilder sbm = new StringBuilder();
	   
		
		for(WifiInfo dp : data) {
		    
			sbs.append(dp.toString());
			
			sbm.append(dp.toCSVString());
		}

		//Log.d(TAG, "Updating UI...");
	//	rssiminer.update(sbs.toString());

		if(locate.toFile) {				
			try {
				File dataDir = new File("/sdcard/rssiWmich/");
				dataDir.mkdirs();
				File dataFile = new File(dataDir,locate.fid);
				Log.d(TAG,"Printing to file /sdcard/rssiWmich/" + locate.fid);
				
				FileWriter fw = new FileWriter(dataFile,true);
                fw.write("Add,");
				fw.write(locate.fingerprintName()+",");
				fw.write(locate.mapx + "," + locate.mapy + ",");
				fw.write(sbm.toString());
				fw.close();

			} catch(FileNotFoundException e) {
				System.err.println("Error: FileNotFoundException\n" + e.getStackTrace());
				System.exit(1);
			} catch (IOException e) {
				System.err.println("Error: IOException\n" + e.getStackTrace());
				System.exit(1);
			} 


		}
	

	}
}
