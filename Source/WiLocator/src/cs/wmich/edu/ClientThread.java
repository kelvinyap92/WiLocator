package cs.wmich.edu;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.net.SocketTimeoutException;
import java.net.UnknownHostException;

import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.util.Log;

/**
 * This class creates thread and handle message between the client and the server
 * @author kelvinyap
 *
 */
public class ClientThread implements Runnable {
	// For debug
	private final String TAG = "ClientThread";

	private Socket socket;
	private String ip;
	private int port;
	private Handler receiveHandler;
	public Handler sendHandler;
	BufferedReader bufferedReader;
	private InputStream inputStream;
	private OutputStream outputStream;
	public boolean isConnect = false;

	private int x;

	private int y;

	
	/**
	 * Client Thread constructor
	 * @param handler
	 * @param ip
	 * @param port
	 */
	public ClientThread(Handler handler, String ip, String port) {
		// TODO Auto-generated constructor stub
		this.receiveHandler = handler;
		this.ip = ip;
		this.port = Integer.valueOf(port);
		Log.d(TAG, "ClientThread's construct is OK!!");
	}

	public ClientThread() {
		Log.d(TAG, "It is may be construct's problem...");
	}

	public void run() {
		try {
			Log.d(TAG, "Into the run()");
			socket = new Socket(ip, port);
			isConnect = socket.isConnected();
			inputStream = socket.getInputStream();
			outputStream = socket.getOutputStream();

			/**
			 * To retrieve message from the server
			 */
			new Thread() {
				@Override
				public void run() {
					byte[] buffer = new byte[4 * 1024];

					final StringBuilder stringBuilder = new StringBuilder();
					try {

						while (socket.isConnected()) {
							int readSize = inputStream.read(buffer);

							Log.d(TAG, "readSize:" + readSize);

							// If Server is stopping
							if (readSize == -1) {
								inputStream.close();
								outputStream.close();
							}
							if (readSize == 0)
								continue;

							// flush buffer
							// Update the receive editText

							stringBuilder.setLength(0);

							stringBuilder
									.append(new String(buffer, 0, readSize));

							Message msg = new Message();
							msg.what = 0x123;
							msg.obj = stringBuilder.toString();
							receiveHandler.sendMessage(msg);

						}

					} catch (IOException e) {
						Log.d(TAG, e.getMessage());
						e.printStackTrace();
					}
				}

			}.start();

			/**
			 * To send message to the server
			 */
			Looper.prepare();
			sendHandler = new Handler() {
				@Override
				public void handleMessage(Message msg) {
					if (msg.what == 0x852) {
						try {
							outputStream.write((msg.obj.toString() + "\r\n")
									.getBytes());
							outputStream.flush();
						} catch (Exception e) {
							Log.d(TAG, e.getMessage());
							e.printStackTrace();
						}
					}
				}
			};
			Looper.loop();

		} catch (SocketTimeoutException e) {
			// TODO Auto-generated catch block
			Log.d(TAG, e.getMessage());
			e.printStackTrace();
		} catch (UnknownHostException e) {
			// TODO Auto-generated catch block
			Log.d(TAG, e.getMessage());
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			Log.d(TAG, e.getMessage());
			e.printStackTrace();
		}
	}

	/**
	 * A method to get the coordinate x receive from the server
	 * 
	 * @return x coordinates
	 */
	public int getCoordinatesX() {

		try {
			Log.d(TAG, "Into the run()");
			socket = new Socket(ip, port);
			isConnect = socket.isConnected();
			inputStream = socket.getInputStream();
			outputStream = socket.getOutputStream();

			// To monitor if receive Msg from Server
			new Thread() {
				@Override
				public void run() {
					byte[] buffer = new byte[4 * 1024];

					final StringBuilder stringBuilder = new StringBuilder();
					try {

						while (socket.isConnected()) {
							int readSize = inputStream.read(buffer);

							Log.d(TAG, "readSize:" + readSize);

							// If Server is stopping
							if (readSize == -1) {
								inputStream.close();
								outputStream.close();
							}
							if (readSize == 0)
								continue;

							// flush buffer
							// stringBuilder.delete(0, readSize);
							// Update the receive editText

							stringBuilder.setLength(0);

							stringBuilder
									.append(new String(buffer, 0, readSize));

							Message msg = new Message();
							msg.what = 0x123;
							msg.obj = stringBuilder.toString();
							receiveHandler.sendMessage(msg);

							// get coordinates
							String[] stuff = stringBuilder.toString()
									.split(",");

							// x = Integer.parseInt(stuff[1]);
							x = Integer.parseInt(stuff[1]);

						}

					} catch (IOException e) {
						Log.d(TAG, e.getMessage());
						e.printStackTrace();
					}
				}

			}.start();

		} catch (SocketTimeoutException e) {
			// TODO Auto-generated catch block
			Log.d(TAG, e.getMessage());
			e.printStackTrace();
		} catch (UnknownHostException e) {
			// TODO Auto-generated catch block
			Log.d(TAG, e.getMessage());
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			Log.d(TAG, e.getMessage());
			e.printStackTrace();
		}

		return x;

	}
	
	/**
	 * A method to retrive coordinate y from the server
	 * 
	 * @return coordinate y
	 */

	public int getCoordinatesY() {

		try {
			Log.d(TAG, "Into the run()");
			socket = new Socket(ip, port);
			isConnect = socket.isConnected();
			inputStream = socket.getInputStream();
			outputStream = socket.getOutputStream();

			// To monitor if receive Msg from Server
			new Thread() {

				@Override
				public void run() {
					byte[] buffer = new byte[4 * 1024];

					final StringBuilder stringBuilder = new StringBuilder();
					try {

						while (socket.isConnected()) {
							int readSize = inputStream.read(buffer);

							Log.d(TAG, "readSize:" + readSize);

							// If Server is stopping
							if (readSize == -1) {
								inputStream.close();
								outputStream.close();
							}
							if (readSize == 0)
								continue;

							// flush buffer
							// stringBuilder.delete(0, readSize);
							// Update the receive editText

							stringBuilder.setLength(0);

							stringBuilder
									.append(new String(buffer, 0, readSize));

							Message msg = new Message();
							msg.what = 0x123;
							msg.obj = stringBuilder.toString();
							receiveHandler.sendMessage(msg);

							// get coordinates
							String[] stuff = stringBuilder.toString()
									.split(",");

							// x = Integer.parseInt(stuff[1]);
							y = Integer.parseInt(stuff[2]);
							System.out.println(y);

						}

					} catch (IOException e) {
						Log.d(TAG, e.getMessage());
						e.printStackTrace();
					}
				}

			}.start();

		} catch (SocketTimeoutException e) {
			// TODO Auto-generated catch block
			Log.d(TAG, e.getMessage());
			e.printStackTrace();
		} catch (UnknownHostException e) {
			// TODO Auto-generated catch block
			Log.d(TAG, e.getMessage());
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			Log.d(TAG, e.getMessage());
			e.printStackTrace();
		}

		return y;

	}

}
