import "./ActorViewer.css"
import { useState, forwardRef, useImperativeHandle, useRef, useEffect } from "react";
import { Pagination, Button, Spin, Modal, Descriptions, Input, message, Card } from 'antd';
import { HeartFilled, HeartOutlined, PlusCircleOutlined } from '@ant-design/icons';
import { getActorByName, likeActor, getMoivesByFilter, createPotPlayerPlayListByActors, getActorByNames } from "../services/DataService";
import MovieViewer from "./MovieViewer";
import { ACTOR_CARD_EACH_PAG } from "../Constant";

const { Search } = Input;
const { Meta } = Card;

const ActorViewer = forwardRef((props, ref) => {
    const [minValue, setMinValue] = useState(0);
    const [maxValue, setMaxValue] = useState(ACTOR_CARD_EACH_PAG);
    const [currentPage, setCurrentPage] = useState(-1);
    const [actorNames, setActorNames] = useState([]);
    const [actor, setActor] = useState(null);
    const [currentPageActorCardDetails, setCurrentPageActorCardDetails] = useState([])
    const [isLoading, setIsLoading] = useState(true);
    const [visible, setVisible] = useState(false);
    const [isLikeLoading, setIsLikeLoading] = useState(false);
    const [likeFlag, setLikeFlag] = useState(0);

    const movieViewer = useRef();

    useImperativeHandle(ref, () => ({
        initializeActors(actorNames) {
            init(actorNames);
        },
        setIsLoading() {
            setIsLoading(true);
        }
    }));

    useEffect(() => {
        if (actorNames.length > 0) {
          setIsLoading(true);
          getActorByNames(actorNames.slice(minValue, maxValue))
            .then((resp) => {
              setCurrentPageActorCardDetails(resp);
              setIsLoading(false);
            })
            .catch((error) => {
              console.log(error);
              setIsLoading(false);
            });
        }
      }, [currentPage]);

    function init(actorNames) {
        setCurrentPage(-1)
        setMinValue(0);
        setMaxValue(ACTOR_CARD_EACH_PAG);
        setActorNames(actorNames);
        setCurrentPage(1)
        setIsLoading(false);
    }

    function handleChange(value) {
        setMinValue((value - 1) * ACTOR_CARD_EACH_PAG);
        setMaxValue(value * ACTOR_CARD_EACH_PAG);
        setCurrentPage(value);
    };

    function showActorDetails(actorIndex) {
        getActorByName(currentPageActorCardDetails[actorIndex].name).then(resp => {
            setActor(resp[0]);
            setVisible(true);
            movieViewer?.current.setIsLoading();
            setLikeFlag(resp[0].liked);
            getMoivesByFilter(0, [resp[0].name], false).then(resp => {
                movieViewer?.current.initializeMovies(resp, 5, actorNames[actorIndex]);
            });
        }).catch((error) => {
            console.log(error);
        });
    }

    function onSearch(value) {
        setIsLoading(true);
        setCurrentPage(-1);
        getActorByName(value).then(resp => {
            let actors = resp ? resp.map(x => x.name) : [];
            init(actors);
            setCurrentPage(1)
        }).catch(error => console.log(error));
    }

    function onLikeClick() {
        setIsLikeLoading(true);
        likeActor(actor?.name).then(resp => {
            setIsLikeLoading(false);
            setLikeFlag(resp);
        }).catch(error => console.log(error));
    }

    function createPotPlayList() {
        let actorslist = [];
        for (let i = 0; i < actorNames.length; ++i) {
            actorslist.push(actorNames[i]);
        }
        createPotPlayerPlayListByActors(actorslist, "Selected Actors").then(() => {
            message.info("加入完毕");
        }).catch((error) => {
            console.log(error);
            message.info("加入失败!");
        });
    }

    return (
        <div className="actor-viewer">
            {isLoading ? <div><Spin size="large" /></div> :
                <Pagination
                    simple
                    current={currentPage}
                    defaultPageSize={ACTOR_CARD_EACH_PAG} //default size of page
                    onChange={handleChange}
                    total={actorNames?.length}
                    className="header-left"
                />}
            <div className="header-right">
                <Search placeholder="演员名" onSearch={onSearch} className="header-element-right actor-search-bar" loading={isLoading} />
                <Button
                    type="primary"
                    icon={<PlusCircleOutlined />}
                    disabled={actorNames?.length === 0 || isLoading ? true : false}
                    onClick={createPotPlayList}
                    className="header-element-right">
                    加入PotPlayer列表
                </Button>
            </div>
            {isLoading ? <div><Spin size="large" /></div> :
                <div>
                    <div className="actor-list">
                        {currentPageActorCardDetails?.map((actor, i) =>
                            <Card
                                hoverable
                                key={"actor-" + i}
                                className="actor-poster-card"
                                onClick={() => showActorDetails(i)}
                                cover={<img className="actor-image" src={actor?.figureSmallPath !== "" ? actor?.figureSmallPath : require('../Imgs/NotFound.jpg')} />}
                            >
                                <Meta title={actor?.name} />
                            </Card>)}
                    </div>
                </div>}
            <Modal
                title={[<Button key="actor-like-btn"
                    shape="circle"
                    icon={likeFlag === true ? <HeartFilled /> : <HeartOutlined />}
                    onClick={onLikeClick}
                    loading={isLikeLoading}></Button>]}
                centered
                visible={visible}
                onOk={() => setVisible(false)}
                onCancel={() => setVisible(false)}
                width={1600}
                className="actor-details"
            >
                <div className="left-container">
                    <Card
                        hoverable
                        cover={<img src={actor?.figureLargePath !== "" ? actor?.figureLargePath : require('../Imgs/NotFound.jpg')} />}
                        className="actor-detail-card"
                    ></Card>
                </div>
                <div className="right-container">
                    <Descriptions title={actor?.name} bordered>
                        <Descriptions.Item label="姓名" span={2}>{actor?.name}</Descriptions.Item>
                        <Descriptions.Item label="生日">{actor?.dateofBirth}</Descriptions.Item>
                        <Descriptions.Item label="身高" span={2}>{actor?.height}</Descriptions.Item>
                        <Descriptions.Item label="罩杯">{actor?.cup}</Descriptions.Item>
                        <Descriptions.Item label="胸围" span={1}>{actor?.bust ?? "?"} cm</Descriptions.Item>
                        <Descriptions.Item label="腰围" span={1}>{actor?.waist ?? "?"} cm</Descriptions.Item>
                        <Descriptions.Item label="臀围" span={1}>{actor?.hips ?? "?"} cm</Descriptions.Item>
                        <Descriptions.Item label="颜值" span={1}>{actor?.looks ?? "?"} 分</Descriptions.Item>
                        <Descriptions.Item label="身材" span={1}>{actor?.body ?? "?"} 分</Descriptions.Item>
                        <Descriptions.Item label="色情" span={1}>{actor?.sexAppeal ?? "?"} 分</Descriptions.Item>
                        <Descriptions.Item label="总体">{actor?.overall ?? "?"} 分</Descriptions.Item>
                    </Descriptions>
                    <MovieViewer ref={movieViewer} searchString2={actor?.name} searchType="Actor" />
                </div>
            </Modal>
        </div>
    )
});
export default ActorViewer;