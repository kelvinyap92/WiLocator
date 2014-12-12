package cs.wmich.edu.wifi;

/**
 * Contains the wifi model information (Signal strength, MAC ADDRESS)
 * @author kelvinyap
 * 
 */
public class WifiInfo {
	int level;
	String BSSID;
	String SSID;

	public WifiInfo(int level, String BSSID, String SSID) {
		this.level = level;

		this.BSSID = BSSID;
		this.SSID = SSID;
	}

	// debug purposes
	@Override
	public String toString() {
		StringBuilder sb = new StringBuilder();

		sb.append("SSID: ").append(SSID).append("\n");
		sb.append("BSSID: ").append(BSSID).append("\n");
		sb.append("RSSI: ").append(level).append(" [dBm]\n\n");

		return sb.toString();
	}

	public String toCSVString() {
		StringBuilder sb = new StringBuilder();

		sb.append(",").append(BSSID).append(",");
		sb.append(level).append("");

		return sb.toString();
	}

}
