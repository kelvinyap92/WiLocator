package cs.wmich.edu;


import cs.wmich.edu.ClientThread;


import android.app.Activity;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;




public class Settings extends Activity   {
	
//tcp stuff here
	 EditText edit_ip = null;
	 EditText edit_port = null;
	 Button btn_connect = null;
 	//About the socket
 	Handler handler;
 	ClientThread clientThread;
	
 	
//end tcp stuff
 	
 	public Settings () {
 		
 	}
	
	
	 
	 @Override
	 public void onCreate(Bundle savedInstanceState) {
	 super.onCreate(savedInstanceState);
	 setContentView(R.layout.settings);
   
	
	 
	 //tcp stuff
	 edit_ip = (EditText) this.findViewById(R.id.edit_ip);
     edit_port = (EditText) this.findViewById(R.id.edit_port);
     btn_connect = (Button) this.findViewById(R.id.btn_connect);

	 

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
				}});
			
			
	 
		 
	
	 
	 }
	 public void init() {
			//Load the datas from share preferences
			SharedPreferences sharedata = getSharedPreferences("data", 0);
			String ip = sharedata.getString("ip", "192.168.");
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
					
						//edit_receive.setText("\n" + msg.obj.toString());
						Toast.makeText(getBaseContext(), msg.obj.toString() ,Toast.LENGTH_SHORT).show();
						
					
					}
				}
			};		
		}
	
	 

	 

}
