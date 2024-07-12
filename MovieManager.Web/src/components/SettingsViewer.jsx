import "./SettingsViewer.css";
import React, { useState, useEffect, forwardRef, useImperativeHandle } from "react";
import { Button, Space, Spin, Input, Form, message, Modal, Switch } from "antd";
import { MinusCircleOutlined, PlusOutlined } from "@ant-design/icons";
import { getUserSettings, scanAndAddNewMovies, deleteMovies, updateUserSettings } from "../services/DataService";

const { TextArea } = Input;

const SettingsViewer = forwardRef((props, ref) => {
    const [isLoading, setIsLoading] = useState(false);
    const [userSettings, setUserSettings] = useState("");
    const [form] = Form.useForm();
    const [messageApi, contextHolder] = message.useMessage();
    const [isFormDirty, setIsFormDirty] = useState(false);
    const [isAddMovieCompleteModalVisible, setIsAddMovieCompleteModalVisible] = useState(false);
    const [addedMovieCount, setAddedMovieCount] = useState(0);
    const [switchState, setSwitchState] = useState(true);
    const sleep = (ms) => new Promise(resolve => setTimeout(resolve, ms));
    
    useEffect(() => {
        if (!userSettings) {
            getUserSettings().then(resp => {
                setUserSettings(resp);
                const directoriesArray = resp.movieDirectory.split("|");
                form.setFieldsValue({
                    movieDirectories: directoriesArray.map(dir => ({ movieDirectory: dir })),
                    actorFiguresDMMDirectory: resp.actorFiguresDMMDirectory,
                    actorFiguresAllDirectory: resp.actorFiguresAllDirectory,
                    potPlayerDirectory: resp.potPlayerDirectory,
                    scanDays: "180"
                });
            });
        }
    }, [userSettings, form]);

    useImperativeHandle(ref, () => ({
        // Define functions to be exposed to parent components here if needed
    }));

    const onFinish = async (values) => {
        let dir = values.movieDirectories.map(item => item.movieDirectory);
        setIsLoading(true);
        userSettings.movieDirectory = dir.join("|");
        userSettings.actorFiguresDMMDirectory = values.actorFiguresDMMDirectory;
        userSettings.actorFiguresAllDirectory = values.actorFiguresAllDirectory;
        userSettings.potPlayerDirectory = values.potPlayerDirectory;
        setUserSettings(userSettings);   
        try {
            await updateUserSettings(userSettings);
            setIsLoading(false);
            messageApi.info("已保存，若改动文件夹请手动刷新页面。");
            setIsFormDirty(false);
            // await sleep(1000);
            // window.location.reload();
        } catch (error) {
            messageApi.error("保存失败！");
            console.log(error);
            setIsLoading(false);
        }
    };

    const addNewMovies = () => { 
        if(form.getFieldValue("movieDirectories")) {
            setIsLoading(true);
            const scanDays = switchState ? -1 : form.getFieldValue("scanDays");
            scanAndAddNewMovies(scanDays).then(resp => {
                messageApi.info("已添加 " + resp + " 部电影！");
                setAddedMovieCount(resp);
                setIsAddMovieCompleteModalVisible(true);
                setIsLoading(false);          
            });
        } else {
            messageApi.error("请先添加电影文件夹！");
        }
    };

    const deleteNonExistMovies = () => {
        if(form.getFieldValue("movieDirectories")) {
            setIsLoading(true);
            deleteMovies().then(resp => {
                messageApi.info("已删除 " + resp?.length + " 电影，请刷新页面!");
                setAddedMovieCount(resp);
                setIsAddMovieCompleteModalVisible(true);
                setIsLoading(false);
            });
        } else {
            messageApi.error("请先添加电影文件夹！");
        }            
    };

    const handleAdd = () => {
        const currentFields = form.getFieldValue("movieDirectories") || [];
        const nextFields = [...currentFields, { movieDirectory: "" }];
        form.setFieldsValue({ movieDirectories: nextFields });
    };

    const handleRemove = (index) => {
        const currentFields = form.getFieldValue("movieDirectories") || [];
        if (currentFields.length > 1) {
            const nextFields = currentFields.filter((_, i) => i !== index);
            form.setFieldsValue({ movieDirectories: nextFields });
        }
    };

    const handleFormChange = () => {
        const isDirty = form.isFieldsTouched();
        setIsFormDirty(isDirty);
    };

    const handleOk = () => {
        setIsAddMovieCompleteModalVisible(false);
        window.location.reload();
    };

    const handleSwitchChange = (checked) => {
        setSwitchState(checked);
    };

    return (
        <div className="settings">
            {contextHolder}
            <Space direction="vertical">
                <Form 
                    form={form} 
                    name="dynamic_form_nest_item" 
                    onFinish={onFinish} 
                    autoComplete="off" 
                    onFieldsChange={handleFormChange}
                    initialValues={{ scanDays: "180" }} 
                >
                    <Form.List name="movieDirectories">
                        {(fields) => (
                            <>
                                {fields.map(({ key, name, fieldKey, ...restField }, index) => (
                                    <Form.Item
                                        {...(index === 0 ? {
                                            labelCol: {
                                                xs: { span: 24 },
                                                sm: { span: 4 },
                                            },
                                            wrapperCol: {
                                                xs: { span: 24 },
                                                sm: { span: 20 },
                                            },
                                        } : {
                                            wrapperCol: {
                                                xs: { span: 24, offset: 0 },
                                                sm: { span: 20, offset: 4 },
                                            },
                                        })}
                                        label={index === 0 ? "文件夹" : ""}
                                        required={false}
                                        key={key}
                                    >
                                        <Form.Item
                                            {...restField}
                                            name={[name, "movieDirectory"]}
                                            rules={[{ required: true, message: "请输入影片位置，或删除这一行" }]}
                                            noStyle
                                        >
                                            <Input placeholder="请输入文件夹位置，如C:\影片文件夹" style={{ width: "80%", marginRight: 8 }} />
                                        </Form.Item>
                                        {fields.length > 1 ? (
                                            <MinusCircleOutlined
                                                className="dynamic-delete-button"
                                                onClick={() => handleRemove(index)}
                                            />
                                        ) : null}
                                    </Form.Item>
                                ))}
                                <Form.Item
                                    wrapperCol={{
                                        xs: { span: 24, offset: 0 },
                                        sm: { span: 20, offset: 4 },
                                    }}
                                >
                                    <Button
                                        type="dashed"
                                        onClick={handleAdd}
                                        style={{ width: "80%" }}
                                        icon={<PlusOutlined />}
                                    >
                                        增加文件夹
                                    </Button>
                                </Form.Item>
                            </>
                        )}
                    </Form.List>
                    <Form.Item
                        label="演员头像（DMM）"
                        name="actorFiguresDMMDirectory"
                        rules={[{ required: false }]}
                        wrapperCol={{
                            xs: { span: 24, offset: 0 },
                            sm: { span: 20, offset: 0 },
                        }}
                    >
                        <Input placeholder="C:\文件夹" style={{ width: "150%", marginRight: 8 }} />
                    </Form.Item>
                    <Form.Item
                        label="演员头像（全体）"
                        name="actorFiguresAllDirectory"
                        rules={[{ required: false }]}
                        wrapperCol={{
                            xs: { span: 24, offset: 0 },
                            sm: { span: 20, offset: 0 },
                        }}
                    >
                        <Input placeholder="C:\Program Files\DAUM\PotPlayer\PotPlayerMini64.exe" style={{ width: "150%", marginRight: 8 }} />
                    </Form.Item>
                    <Form.Item
                        label="PotPlayer文件夹"
                        name="potPlayerDirectory"
                        rules={[{ required: true }]}
                        wrapperCol={{
                            xs: { span: 24, offset: 0 },
                            sm: { span: 20, offset: 0 },
                        }}
                    >
                        <Input placeholder="C:\文件夹" style={{ width: "150%", marginRight: 8 }} />
                    </Form.Item>
                    { !switchState && (
                    <Form.Item
                        label="扫描多少天内添加的电影"
                        name="scanDays"
                        rules={[{ required: false }]}
                        wrapperCol={{
                            xs: { span: 24, offset: 0 },
                            sm: { span: 20, offset: 0 },
                        }}
                    >
                        <Input style={{ width: "100%", marginRight: 8 }} />
                    </Form.Item>
                    )}
                    <Form.Item
                        label="扫描所有文件"
                        valuePropName="checked"
                        wrapperCol={{
                            xs: { span: 24, offset: 0 },
                            sm: { span: 20, offset: 4 },
                        }}
                    >
                        <Switch onChange={handleSwitchChange} defaultChecked={true} />
                    </Form.Item>
                    <Form.Item
                        wrapperCol={{
                            xs: { span: 24, offset: 0 },
                            sm: { span: 20, offset: 4 },
                        }}
                    >
                        <Spin size="large" tip="加载中..." spinning={isLoading}>
                            <Space>
                                <Button type="primary" shape="round" htmlType="submit">
                                    保存设置
                                </Button>
                                <Button type="primary" shape="round" onClick={addNewMovies} disabled={isFormDirty}>
                                    添加新电影
                                </Button>
                                <Button type="primary" shape="round" onClick={deleteNonExistMovies} disabled={isFormDirty}>
                                    移除已删除的电影
                                </Button>
                            </Space>
                        </Spin>
                    </Form.Item>
                </Form>
                <Modal 
                    title="已完成添加电影"
                    visible={isAddMovieCompleteModalVisible}
                    onOk={handleOk}
                    footer={[
                        <Button key="submit" type="primary" onClick={handleOk}>
                            刷新页面
                        </Button>
                    ]}
                >
                    {`已添加 ${addedMovieCount} 部电影！`} 
                </Modal>
            </Space>
        </div>
    );
});

export default SettingsViewer;