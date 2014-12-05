package cs.wmich.edu;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Timer;
import java.util.TimerTask;

import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.net.wifi.ScanResult;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.Menu;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnTouchListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.ToggleButton;
import cs.wmich.edu.wifi.LocateReceiver;
import cs.wmich.edu.wifi.WifiInfo;
import cs.wmich.edu.wifi.WifiReceiver;


public class Locate extends Activity implements OnClickListener,OnTouchListener  {
	
	//tcp stuff here
		 EditText edit_ip = null;
		 EditText edit_port = null;
		 Button btn_connect = null;
	    Button send = null;
		//About the socket
		Handler handler;
		ClientThread clientThread;
		
	//end tcp stuff
		
		
		Button Locate;
		Button Remove;
		//Button scanButton;

		
	
	private static final String TAG = "RSSIMiner";
	public WifiManager WiFi;
	BroadcastReceiver receiver;



	public boolean toFile = false;
	public String fid = "0";

	Timer tim;
	boolean scanning = false;
	
	EditText mEdit;
	
	
	//=============================
	// The graphic components of the app
	//	ToggleButton setPoint;
		ImageView map;
		ImageView red_map_pin;
		Boolean activeMode;
		static ImageView map_pin;
		
		// Location x, y on the map
		public int mapx;
		public int mapy;
		
		// Map dimension in pixel
		int mapwidth;
		int mapheight;
		
		// Location x, y in the actual room in cm
		int roomx;
		int roomy;
		
		// Room dimension in cm
		int roomwidth = 300;
		int roomheight = 600;
//======================================

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.locate);

		// Setup UI
		//textStatus = (TextView) findViewById(R.id.textStatus);

		 //tcp stuff
		 edit_ip = (EditText) this.findViewById(R.id.edit_ip);
	     edit_port = (EditText) this.findViewById(R.id.edit_port);
	     btn_connect = (Button) this.findViewById(R.id.btn_connect);
	     send = (Button) this.findViewById(R.id.send);
       //end tcp stuff
	     
	 	//Locate = (Button) findViewById(R.id.locate);
		//Locate.setOnClickListener(this);
		
		//scanButton = (Button) findViewById(R.id.buttonScan);
		//scanButton.setOnClickListener(this);
		
		Remove = (Button) findViewById(R.id.remove);
		Remove.setOnClickListener(this);
		
		init();

	     //Click here to connect
			btn_connect.setOnClickListener(new OnClickListener(){

				@Override
				public void onClick(View arg0) {
					// TODO Auto-generated method stub
					String ip = edit_ip.getText().toString();
					String port = edit_port.getText().toString();
					
					//Log.d(TAG, ip + port);
					
					clientThread = new ClientThread(handler, ip, port);
					new Thread(clientThread).start();
					//Log.d(TAG, "clientThread is start!!");
					if(clientThread.isConnect)
					{
						btn_connect.setText(R.string.btn_disconnect);
					}
				}});//end connect
			
//			Locate.setOnClickListener(new OnClickListener() {
//				 
//				 public void onClick(View v) {
//					 try{
//						
//						 
//						   WiFi.startScan();
//
//			
//						 
//						 List<ScanResult> results = WiFi.getScanResults();
//						 ArrayList<WifiInfo> data = new ArrayList<WifiInfo>(results.size());
//							for (ScanResult result : results.subList(0, 15)) {
//								
//								data.add(new WifiInfo(result.level, result.BSSID, result.SSID));
//							}
//							StringBuilder sbm = new StringBuilder();
//						     for(WifiInfo dp : data) {
//							   sbm.append(dp.toCSVString());
//							}
//						     
//						 //ip start
//							Message msg = new Message();
//							msg.what = 0x852;
//							msg.obj = "locate"+sbm.toString();
//						
//						
//							clientThread.sendHandler.sendMessage(msg);
//						
//							//textmsg.setText("");
//							
//						
//						
//                           setLocation(clientThread.getCoordinatesX(), clientThread.getCoordinatesY());
//							 
//						
//						 
//						 
//					 }catch (Exception E) {
//						 Toast.makeText(getBaseContext(), E.getMessage(),
//									Toast.LENGTH_SHORT).show();
//					 }
//					
//				 }
//				 
//			 }); // locate
			
		
			
			
			send.setOnClickListener(new OnClickListener() {
				 
				 public void onClick(View v) {
					 try{
				
						 //ip start
							Message msg = new Message();
							msg.what = 0x852;
							msg.obj = "emergency,"+fingerprintName()+","+mapx+","+mapy;
						
						
							clientThread.sendHandler.sendMessage(msg);
						
							//textmsg.setText("");
						 
					
					 }catch (Exception E) {
						 Toast.makeText(getBaseContext(), E.getMessage(),
									Toast.LENGTH_SHORT).show();
					 }
					
				 }
				 
			 }); // send
			
			Remove.setOnClickListener(new OnClickListener() {
				 
				 public void onClick(View v) {
					 try{
				
						     
						 //ip start
							Message msg = new Message();
							msg.what = 0x852;
							msg.obj = "remergency,"+fingerprintName();
						
						
							clientThread.sendHandler.sendMessage(msg);
						
							//textmsg.setText("");
						 
					
					 }catch (Exception E) {
						 Toast.makeText(getBaseContext(), E.getMessage(),
									Toast.LENGTH_SHORT).show();
					 }
					
				 }
				 
			 }); // remove

		// Setup WiFi
		WiFi = (WifiManager) getSystemService(Context.WIFI_SERVICE);

		// Setup Timer
		tim = new Timer(true);

		// Register Broadcast Receiver
		if (receiver == null)
			receiver = new LocateReceiver(this);

		registerReceiver(receiver, new IntentFilter(
				WifiManager.SCAN_RESULTS_AVAILABLE_ACTION));
		Log.d(TAG, "onCreate()");
		
		//==================================
	    // Initializing variables 
      //  setPoint = (ToggleButton) findViewById(R.id.setPointButton);
        map = (ImageView) findViewById(R.id.map);
        map.setOnTouchListener(this);

        red_map_pin = (ImageView) findViewById(R.id.red_map_pin);
        red_map_pin.setVisibility(View.INVISIBLE);
        
        map_pin = (ImageView) findViewById(R.id.map_pin);
        map_pin.setVisibility(View.VISIBLE);
        
	}


	private void init() {
		//Load the datas from share preferences
				SharedPreferences sharedata = getSharedPreferences("data", 0);
				String ip = sharedata.getString("ip", "");
				String port = sharedata.getString("port", "5000");
				edit_ip.setText(ip);
				edit_port.setText(port);
				
				handler = new Handler()
				{
					@Override
					public void handleMessage(Message msg)
					{
						if(msg.what == 0x123)
						{
							//msg.recycle();

							Toast.makeText(getBaseContext(), msg.obj.toString() ,Toast.LENGTH_SHORT).show();
					        


							//edit_receive.setText("\n" + msg.obj.toString());
						}
					}
				};
		
	}


	@Override
	public void onDestroy() {
		Log.d(TAG,"onDestroy()");
		this.unregisterReceiver(receiver);
		super.onDestroy();
	}
	

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}
	


	
	public String fingerprintName(){
		mEdit   = (EditText)findViewById(R.id.fingerprintName);
		return mEdit.getText().toString();
	}
	
	 public boolean onTouch(View v, MotionEvent event) {
	    	// If we are in active mode
	    
		    	// Get the dimensions of the map
		        mapwidth = map.getMeasuredWidth();
		        mapheight = map.getMeasuredHeight();
				
				// retrieve event coordinates
				mapx = (int)event.getX();
				mapy = (int)event.getY();
				
				// Display the map_pin to the event location (substract map_pin dimensions)
				red_map_pin.setPadding(mapx-8, mapy-28,0,0);
				red_map_pin.setVisibility(View.VISIBLE);
				red_map_pin.bringToFront();
				
				// Compute actual location in the room in cm
				roomx = mapwidth * mapx / roomwidth;
				roomy = mapheight * mapy / roomheight;
							
			return false;
		}
	 
	 public String getCoordinate() {
		 String coordinatex = Integer.toString(mapx);
		 																	
		 return coordinatex ;
	 }
	    
//	    public void setPointToggleSwitch(View v){
//	    	// When mode switches
//	    	if (setPoint.isChecked()){
//	    		// If turning on
//	    
//	    		activeMode = true;
//	    	}else{
//	    		// If turning off
//	    	
//	    		activeMode = false;
//	    	}
//	    }


		@Override
		public void onClick(View v) {
			// TODO Auto-generated method stub
			
		}
	    
	    
		 // To set the pin on the map with it's coordinates
	    public static void setLocation(int x, int y){
	    	map_pin.setPadding(x-8, y-28,0,0);
			map_pin.setVisibility(View.VISIBLE);
			map_pin.bringToFront();
	    }
		
	

}
