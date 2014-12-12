package cs.wmich.edu;

import java.util.ArrayList;
import java.util.List;
import java.util.Timer;

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
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;
import cs.wmich.edu.wifi.WifiInfo;

/**
 * A calibrate class written for testing calibration purpose. Methods below are implemented to Setup Fingerprint
 * @author kelvinyap
 *
 */
public class Calibrate extends Activity implements OnClickListener {

	// tcp stuff here
	EditText edit_ip = null;
	EditText edit_port = null;
	Button btn_connect = null;
	Button send = null;
	// About the socket
	Handler handler;
	ClientThread clientThread;

	// end tcp stuff

	private static final String tag = "RSSIMiner";
	public WifiManager WiFi;
	BroadcastReceiver receiver;

	TextView textStatus;
	// Button scanButton;
	// Button recordButton;
	Button recalibrate;

	public boolean toFile = false;
	public String fid = "0";

	Timer tim;
	boolean scanning = false;

	EditText mEdit;

	// ======================================

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.calibrate);

		recalibrate = (Button) findViewById(R.id.reCalibrate);
		recalibrate.setOnClickListener(this);

		// tcp stuff
		edit_ip = (EditText) this.findViewById(R.id.edit_ip);
		edit_port = (EditText) this.findViewById(R.id.edit_port);
		btn_connect = (Button) this.findViewById(R.id.btn_connect);
		send = (Button) this.findViewById(R.id.send);
		// end tcp stuff
		
		// Setup WiFi
		WiFi = (WifiManager) getSystemService(Context.WIFI_SERVICE);

		// Setup Timer
		tim = new Timer(true);

		// Register Broadcast Receiver
		//if (receiver == null)
			//receiver = new CalibrateReceiver(this);

		registerReceiver(receiver, new IntentFilter(
				WifiManager.SCAN_RESULTS_AVAILABLE_ACTION));
		Log.d(tag, "onCreate()");
		
		init();

		// Click here to connect
		btn_connect.setOnClickListener(new OnClickListener() {

			@Override
			public void onClick(View arg0) {
				// TODO Auto-generated method stub
				String ip = edit_ip.getText().toString();
				String port = edit_port.getText().toString();

				// Log.d(TAG, ip + port);

				clientThread = new ClientThread(handler, ip, port);
				new Thread(clientThread).start();
				// Log.d(TAG, "clientThread is start!!");
				if (clientThread.isConnect) {
					btn_connect.setText(R.string.btn_disconnect);
				}
			}
		});// end connect

		send.setOnClickListener(new OnClickListener() {

			public void onClick(View v) {
				try {

					WiFi.startScan();

					List<ScanResult> results = WiFi.getScanResults();
					ArrayList<WifiInfo> data = new ArrayList<WifiInfo>(results
							.size());
					for (ScanResult result : results.subList(0, 15)) {

						data.add(new WifiInfo(result.level, result.BSSID,
								result.SSID));
					}
					StringBuilder sbm = new StringBuilder();
					for (WifiInfo dp : data) {
						sbm.append(dp.toCSVString());
					}

					// ip start
					Message msg = new Message();
					msg.what = 0x852;
					msg.obj = "calibrate," + fingerprintName() + sbm.toString();

					clientThread.sendHandler.sendMessage(msg);

					// textmsg.setText("");

				} catch (Exception E) {
					Toast.makeText(getBaseContext(), E.getMessage(),
							Toast.LENGTH_SHORT).show();
				}

			}

		}); // send

		recalibrate.setOnClickListener(new OnClickListener() {

			public void onClick(View v) {
				try {
					

					WiFi.startScan();

					List<ScanResult> results = WiFi.getScanResults();
					ArrayList<WifiInfo> data = new ArrayList<WifiInfo>(results
							.size());
					for (ScanResult result : results.subList(0, 15)) {

						data.add(new WifiInfo(result.level, result.BSSID,
								result.SSID));
					}
					StringBuilder sbm = new StringBuilder();
					for (WifiInfo dp : data) {
						sbm.append(dp.toCSVString());
					}

					// ip start
					Message msg = new Message();
					msg.what = 0x852;
					msg.obj = "recalibrate," + fingerprintName()
							+ sbm.toString();

					clientThread.sendHandler.sendMessage(msg);

					// textmsg.setText("");

				} catch (Exception E) {
					Toast.makeText(getBaseContext(), E.getMessage(),
							Toast.LENGTH_SHORT).show();
				}

			}

		}); // send

	

		

	}

	private void init() {
		SharedPreferences sharedata = getSharedPreferences("data", 0);

		String ip = sharedata.getString("ip", "");
		String port = sharedata.getString("port", "5000");
		edit_ip.setText(ip);
		edit_port.setText(port);

		handler = new Handler() {
			@Override
			public void handleMessage(Message msg) {
				if (msg.what == 0x123) {
					// edit_receive.setText("\n" + msg.obj.toString());
					Toast.makeText(getBaseContext(), msg.obj.toString(),
							Toast.LENGTH_SHORT).show();

				}
			}
		};

	}

	@Override
	public void onDestroy() {
		Log.d(tag, "onDestroy()");
		this.unregisterReceiver(receiver);
		super.onDestroy();
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}


	public String fingerprintName() {
		mEdit = (EditText) findViewById(R.id.fingerprintName);
		return mEdit.getText().toString();
	}

	@Override
	public void onClick(View v) {
		// TODO Auto-generated method stub

	}

}
