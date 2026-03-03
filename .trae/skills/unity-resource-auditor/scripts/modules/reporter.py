import os
import csv
import datetime

def generate_report_content(report, all_assets):
    lines = []
    lines.append("="*30)
    lines.append(" 资源分析报告")
    lines.append("="*30)
    
    lines.append("\n[ 资源摘要 ]")
    lines.append(f"{'类型':<15} | {'数量':<8} | {'大小 (MB)':<10}")
    lines.append("-" * 38)
    for type_name, count in sorted(report['summary']['count'].items()):
        size = report['summary']['size_mb'].get(type_name, 0)
        lines.append(f"{type_name:<15} | {count:<8} | {size:.2f}")

    lines.append("\n[ 潜在问题 ]")
    
    issue_count = 0
    for asset in all_assets:
        if asset['issues']:
            for issue in asset['issues']:
                lines.append(f"- [{asset['type']}] {issue}: {asset['path']}")
                issue_count += 1
                
    if issue_count == 0:
        lines.append("未发现主要问题。")
        
    lines.append("\n" + "="*30)
    return "\n".join(lines)

def save_reports(report, all_assets, output_base_dir="AnalyzeResources"):
    timestamp = datetime.datetime.now().strftime("%Y-%m-%d-%H-%M-%S")
    report_content = generate_report_content(report, all_assets)
    
    # Print to Console
    print("\n" + report_content)

    # Output Directory
    output_dir = os.path.join(output_base_dir, f"Audit_{timestamp}")
    
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)
        
    # Save Markdown Report
    filename_md = f"ResourceAudit_{timestamp}.md"
    file_path_md = os.path.join(output_dir, filename_md)
    
    try:
        with open(file_path_md, 'w', encoding='utf-8') as f:
            f.write("# Unity Resource Audit Report\n")
            f.write(f"Generated on: {datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n\n")
            f.write("```\n")
            f.write(report_content)
            f.write("\n```\n")
        print(f"\n[Info] Full report saved to: {file_path_md}")
    except Exception as e:
        print(f"\n[Error] Failed to save report file: {str(e)}")

    # Save CSV Report (Excel friendly)
    filename_csv = f"ResourceAudit_{timestamp}.csv"
    file_path_csv = os.path.join(output_dir, filename_csv)
    
    try:
        with open(file_path_csv, 'w', newline='', encoding='utf-8-sig') as f: # utf-8-sig for Excel compatibility
            writer = csv.writer(f)
            writer.writerow(['Path', 'Type', 'Size (MB)', 'Issues'])
            for asset in all_assets:
                issues_str = "; ".join(asset['issues']) if asset['issues'] else ""
                writer.writerow([asset['path'], asset['type'], f"{asset['size_mb']:.4f}", issues_str])
        print(f"[Info] Excel(CSV) report saved to: {file_path_csv}")
    except Exception as e:
        print(f"\n[Error] Failed to save CSV file: {str(e)}")

    # Print output dir for external tools to capture
    print(f"[Output Directory] {output_dir}")
    return output_dir, timestamp
