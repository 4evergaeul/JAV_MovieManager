import "./Settings.css"
import { useState, forwardRef, useImperativeHandle, useEffect } from "react";
import { Button, Space, Spin, Input, Upload } from "antd";
import { FolderOutlined } from '@ant-design/icons';
import { getUserSettings, scanAndAddNewMovies, deleteMovies } from "../services/DataService";

const Settings = forwardRef((props, ref) => {
    const [isLoading, setIsLoading] = useState(false);
    const [userSettings, setUserSettings] = useState("")

    useEffect(() => {
        if(!userSettings) {
            getUserSettings().then(resp => {
                setUserSettings(resp)
            });
        }
    }, [userSettings]);

    useImperativeHandle(ref, () => ({
    }));

    function addNewMovies() {
        setIsLoading(true);
        scanAndAddNewMovies(180).then(() => {
            setIsLoading(false);
        });
    }

    function deleteNonExistMovies() {
        setIsLoading(true);
        deleteMovies().then(() => {
            setIsLoading(false);
        });
    }


    function onFolderSelected(info) {
        console.log(info);
    }

    return (
        <div className="settings">
            <Space direction="vertical">
                <Space>
                    <input type="file" id="folder" webkitdirectory="true" multiple/>
                </Space>
                <Spin size="large" tip="加载中..." spinning={isLoading}>
                    <Space>
                        <Button type="primary" shape="round" onClick={addNewMovies}>添加新电影</Button>
                        <Button type="primary" shape="round" onClick={deleteNonExistMovies}>移除已删除的电影</Button>
                    </Space>
                </Spin>
            </Space>
        </div>
    )
});
export default Settings;