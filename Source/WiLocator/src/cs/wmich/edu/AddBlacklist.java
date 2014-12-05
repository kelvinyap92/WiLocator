package cs.wmich.edu;

import java.io.File;
import java.io.FileOutputStream;
import java.io.OutputStreamWriter;

import cs.wmich.edu.ClientThread;


import android.app.Activity;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.preference.PreferenceManager;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

public class AddBlacklist extends Activity   {
	
//tcp stuff here
	 EditText edit_ip = null;
	 EditText edit_port = null;
	 Button btn_connect = null;
     Button btn_send = null;
 	//About the socket
 	Handler handler;
 	ClientThread clientThread;
 	
//end tcp stuff
	
	 EditText textmsg;
	 
	 //send
	 Button btnAdd;
	 Button btnRemove;
	 
	 @Override
	 protected void onCreate(Bundle savedInstanceState) {
	 super.onCreate(savedInstanceState);
	 setContentView(R.layout.blacklist);
	 
	 textmsg=(EditText)findViewById(R.id.macaddress);
	 textmsg.setHint("Mac Address...");
	 
	 
	 
 //tcp stuff
	 edit_ip = (EditText) this.findViewById(R.id.edit_ip);
     edit_port = (EditText) this.findViewById(R.id.edit_port);
     btn_connect = (Button) this.findViewById(R.id.btn_connect);

	init();
	 
	 btnAdd = (Button) findViewById(R.id.add);
	 
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
			

	 
	   btnAdd.setOnClickListener(new OnClickListener() {
		   
		  
		 
		 public void onClick(View v) {
			 
			 
			 try{
 
				 //ip start
					Message msg = new Message();
					msg.what = 0x852;
					
					msg.obj = "addblack," + textmsg.getText().toString();
				
					clientThread.sendHandler.sendMessage(msg);
					
					textmsg.setText("");
				 
				
			 }catch (Exception E) {
				 Toast.makeText(getBaseContext(), E.getMessage(),
							Toast.LENGTH_SHORT).show();
			 }
			
		 }
		 
	 }); // btnAdd
	 
	 btnRemove = (Button) findViewById(R.id.remove);
     btnRemove.setOnClickListener(new OnClickListener() {
    	 
    	

		@Override
		public void onClick(View v) {
			try{
				
				 //ip start
				Message msg = new Message();
				msg.what = 0x852;
				
				msg.obj = "delblack," + textmsg.getText().toString();
			
				clientThread.sendHandler.sendMessage(msg);
				textmsg.setText("");
			 
				
			 }catch (Exception E) {
				 Toast.makeText(getBaseContext(), E.getMessage(),
							Toast.LENGTH_SHORT).show();
			 }
			
		 }
 	 
     });
	 
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
					//edit_receive.setText("\n" + msg.obj.toString());
					Toast.makeText(getBaseContext(), msg.obj.toString() ,Toast.LENGTH_SHORT).show();
					
				
				}
			}
		};		
	}
	 
	 
	

	 

}
