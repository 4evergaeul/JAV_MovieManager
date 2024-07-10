import './App.css';
import 'antd/dist/antd.css'
import { getAllActorNames, getActorsByRanges, getMoivesByFilter, getMovieYears, getMostRecentMovies, getLikedMovies, getLikedActors, getAllGenreNames, getAllTagNames, getLikedGenres, getLikedTags, getAllDirectorNames } from './services/DataService.js';
import { useEffect, useRef, useState } from "react";
import { Layout, Menu, Slider, Space, Button, Checkbox, message } from 'antd';
import { UserOutlined, GroupOutlined, TagOutlined, CalendarOutlined } from '@ant-design/icons';
import CheckboxTree from "react-checkbox-tree";
import MovieViewer from './components/MovieViewer';
import ActorViewer from './components/ActorViewer';
import SettingsViewer from './components/SettingsViewer';
import GenreViewer from './components/GenreViewer';
import TagViewer from './components/TagViewer';
import DirectorViewer from './components/DirectorViewer.jsx';
import { MOVIE_CARD_EACH_PAGE_LARGE_SCREEN } from './Constant.js';

const { SubMenu } = Menu;
const { Content, Sider } = Layout;

function App() {
  const [allActors, setAllActors] = useState([]);
  const [allGenres, setAllGenres] = useState([]);
  const [allTags, setAllTags] = useState([]);
  const [allDirectors, setAllDirectors] = useState([]);
  const [allYears, setAllYears] = useState([]);
  const [mostRecentMovies, setMostRecentMovies] = useState([]);

  const [selectedActors, setSelectedActors] = useState([]);
  const [selectedGenres, setSelectedGenres] = useState([]);
  const [selectedTags, setSelectedTags] = useState([]);
  const [selectedYears, setSelectedYears] = useState([]);

  const [selectedMenuKey, setSelectedMenuKey] = useState(['1']);

  const [heightRange, setHeightRange] = useState([140, 190]);
  const [cupRange, setCupRange] = useState([1, 26]);
  const [ageUpper, setAgeUpper] = useState(50);

  const movieViewer = useRef();
  const actorViewer = useRef();
  const genreViewer = useRef();
  const tagViewer = useRef();
  const directorViewer = useRef();
  const customSearchViewer = useRef();
  const settings = useRef();
  
  const heightMarks = { 140: "140cm", 190: "190cm" }
  const ageMarks = { 18: "18", 100: "100" }
  const cupMarks = {
    1: "A", 2: "B", 3: "C", 4: "D", 5: "E", 6: "F", 7: "G", 8: "H", 9: "I", 10: "J", 11: "K", 12: "L", 13: "M", 14: "N",
    15: "O", 16: "P", 17: "Q", 18: "R", 19: "S", 20: "T", 21: "U", 22: "V", 23: "W", 24: "X", 25: "Y", 26: "Z"
  }

  useEffect(() => {
    if (allActors?.length === 0) {
      getAllActorNames().then(resp => {
          let results = resp ? getResponses(resp) : [];
          setAllActors(results);
      });
    }
    if (allGenres?.length === 0) {
      getAllGenreNames().then(resp => {
        if(resp) {
          let results = resp ? getResponses(resp) : [];
          setAllGenres(results);
        }
      });
    }
    if (allTags?.length === 0) {
      getAllTagNames().then(resp => {
        let results = resp ? getResponses(resp) : [];
        setAllTags(results);
      });
    }
    if (allDirectors?.length === 0) {
      getAllDirectorNames().then(resp => {
        let results = resp ? getResponses(resp) : [];
        setAllDirectors(results);
      });
    }
    if (allYears?.length === 0) {
      getMovieYears().then(resp => {
        let results = resp ? getResponses(resp) : [];
        setAllYears(results);
      });
    }
    if (mostRecentMovies?.length === 0) {
      getMostRecentMovies().then(resp => {
        setMostRecentMovies(resp);
        movieViewer?.current?.initializeMovies(resp);
        if(!resp) {
          message.info("没有找到影片，请到设置添加影片！");
        }
      });
    }
  }, [allActors], [allGenres], [allTags], [allDirectors], [allYears], [mostRecentMovies]);

  const movieSideBar = (
    <Menu
      mode="inline"
      style={{ height: '100%' }}
    >
      <SubMenu
        key="actors"
        icon={<UserOutlined />}
        title="演员"
        onTitleClick={resetAllSelected}>
        <CheckboxTree
          nodes={allActors}
          checked={selectedActors}
          onCheck={onActorChanged}
        />
      </SubMenu>
      <SubMenu
        key="genres"
        icon={<GroupOutlined />}
        title="类型"
        onTitleClick={resetAllSelected}>
        <CheckboxTree
          nodes={allGenres}
          checked={selectedGenres}
          onCheck={onGenresChanged}
        />
      </SubMenu>
      <SubMenu
        key="tags"
        icon={<TagOutlined />}
        title="标签"
        onTitleClick={resetAllSelected}>
        <CheckboxTree
          nodes={allTags}
          checked={selectedTags}
          onCheck={onTagsChanged}
        />
      </SubMenu>
      <SubMenu
        key="years"
        icon={<CalendarOutlined />}
        title="年份"
        onTitleClick={resetAllSelected}>
        <CheckboxTree
          nodes={allYears}
          checked={selectedYears}
          onCheck={onYearsChanged}
        />
      </SubMenu>
      <Checkbox className="like-checkbox" onChange={e => onMovieLikeCheckboxChange(e)}>喜欢的影片</Checkbox>
    </Menu>
  );

  const actorSideBar = (
    <Space className="sidebar" direction="vertical" size="large">
      <div>
        <p className="actor-sidebar-label">身高</p>
        <Slider
          className="actor-sidebar-slider"
          marks={heightMarks} range
          defaultValue={[140, 190]}
          min={140} max={190} 
          onChange={value => setHeightRange(value)}
          value={heightRange}/>
      </div>
      <div>
        <p className="actor-sidebar-label">年龄</p>
        <Slider className="actor-sidebar-slider"
          marks={ageMarks}
          min={18} max={100} 
          onChange={value => setAgeUpper(value)}
          value={ageUpper}/>
      </div>
      <div>
        <p className="actor-sidebar-label">罩杯</p>
        <Slider className="actor-sidebar-slider"
          marks={cupMarks} range
          min={1} max={26} 
          onChange={value => setCupRange(value)}
          value={cupRange}/>
      </div>
      <div>
        <Checkbox onChange={e => onActorLikeCheckboxChange(e)}>喜欢的演员</Checkbox>
      </div>
      <Space direction="horizontal" size="small">
        <Button type="primary" onClick={onActorFilterApply}>确认</Button>
        <Button onClick={resetActorFilter}>重置</Button>
      </Space>
    </Space>
  );

  const genreSideBar = (
    <Space className="sidebar" direction="vertical" size="large">
      <div>
        <Checkbox onChange={e => onGenreLikeCheckboxChange(e)}>喜欢的类型</Checkbox>
      </div>
    </Space>
  );

  const tagSideBar = (
    <Space className="sidebar" direction="vertical" size="large">
      <div>
        <Checkbox onChange={e => onTagLikeCheckboxChange(e)}>喜欢的标签</Checkbox>
      </div>
    </Space>
  );

  function onActorFilterApply() {
    getActorsByRanges(heightRange[0], heightRange[1], cupMarks[cupRange[0]], cupMarks[cupRange[1]], ageUpper).then(
      async resp => {
        let actorNames = resp;
        actorViewer?.current?.initializeActors(actorNames);
      }).catch(error => console.log(error));
  }

  function resetActorFilter() {
    setHeightRange([140, 190]);
    setAgeUpper(50);
    setCupRange([1, 26]);
  }

  function getResponses(resp) {
    let results = [];
    for (let i = 0; i < resp.length; ++i) {
      results.push({ value: resp[i], label: resp[i] });
    }
    return results;
  }

  function onActorChanged(checkedValues) {
    setSelectedActors(checkedValues);
    loadMoviesByFilter(checkedValues, 0, false);
  }

  function onGenresChanged(checkedValues) {
    setSelectedGenres(checkedValues);
    loadMoviesByFilter(checkedValues, 1, true);
  }

  function onTagsChanged(checkedValues) {
    setSelectedTags(checkedValues);
    loadMoviesByFilter(checkedValues, 2, true);
  }

  function onYearsChanged(checkedValues) {
    setSelectedYears(checkedValues);
    loadMoviesByFilter(checkedValues, 4);
  }

  function loadMoviesByFilter(checkedValues, filterType, isAndOperator) {
    movieViewer?.current.setIsLoading();
    if (checkedValues?.length > 0) {
      getMoivesByFilter(filterType, checkedValues, isAndOperator).then(resp => {
        movieViewer?.current.initializeMovies(resp);
      });
    } else {
      movieViewer?.current.initializeMovies(mostRecentMovies, MOVIE_CARD_EACH_PAGE_LARGE_SCREEN);
    }
  }

  function resetAllSelected() {
    setSelectedActors([]);
    setSelectedGenres([]);
    setSelectedTags([]);
    setSelectedYears([]);
    movieViewer?.current?.initializeMovies(mostRecentMovies, MOVIE_CARD_EACH_PAGE_LARGE_SCREEN);
  }

  function onMenuOptionClicked(e) {
    setSelectedMenuKey(e.key);
    resetActorFilter();
    setTimeout(() => {
      if (e.key[0] === '1') {
        movieViewer?.current.initializeMovies(mostRecentMovies, MOVIE_CARD_EACH_PAGE_LARGE_SCREEN);
      } else if (e.key[0] === '2') {
        actorViewer?.current.initializeActors(allActors.map(x => x.value));
      } else if (e.key[0] === '3') {
        genreViewer?.current.initializeGenres(allGenres.map(x => x.value));
      } else if (e.key[0] === '4') {
        tagViewer?.current.initializeTags(allTags.map(x => x.value));
      } else if (e.key[0] === '5') {
        directorViewer?.current.initializeDirectors(allDirectors.map(x => x.value));
      }
    }, 100);
  }

  function onMovieLikeCheckboxChange(e) {
    movieViewer?.current.setIsLoading();
    if(e.target.checked) {
      getLikedMovies().then(resp => {
        let result = resp ? resp : [];
        movieViewer?.current.initializeMovies(result, MOVIE_CARD_EACH_PAGE_LARGE_SCREEN);
      });
    } else {
      movieViewer?.current.initializeMovies(mostRecentMovies, MOVIE_CARD_EACH_PAGE_LARGE_SCREEN);
    }
  }

  function onActorLikeCheckboxChange(e) {
    actorViewer?.current.setIsLoading();
    if(e.target.checked) {
      getLikedActors().then(resp => {
        let result = resp ?? [];
        actorViewer?.current.initializeActors(result);
      });
    } else {
      actorViewer?.current.initializeActors(allActors.map(x => x.value));
    }
  }

  function onGenreLikeCheckboxChange(e) {
    genreViewer?.current.setIsLoading();
    if(e.target.checked) {
      getLikedGenres().then(resp => {
        genreViewer?.current.initializeGenres(resp);
      });
    } else {
      genreViewer?.current.initializeGenres(allGenres.map(x => x.value));
    }
  }

  function onTagLikeCheckboxChange(e) {
    tagViewer?.current.setIsLoading();
    if(e.target.checked) {
      getLikedTags().then(resp => {
        tagViewer?.current.initializeTags(resp);
      });
    } else {
      tagViewer?.current.initializeTags(allTags.map(x => x.value));
    }
  }

  return (
    <div className="App">
      <Layout>
        <Layout className="header" theme='light'>
          <div className="logo" />
          <Menu theme="light" mode="horizontal" selectedKeys={selectedMenuKey} onClick={onMenuOptionClicked}>
            <Menu.Item key="1">电影</Menu.Item>
            <Menu.Item key="2">演员</Menu.Item>
            <Menu.Item key="3">类型</Menu.Item>
            <Menu.Item key="4">标签</Menu.Item>
            <Menu.Item key="5">导演</Menu.Item>
            <Menu.Item key="6">播放列表</Menu.Item>
            <Menu.Item key="0">设置</Menu.Item>
          </Menu>
        </Layout>
        <Layout>
          <Sider width={300} className="side" theme='light'>
            {selectedMenuKey[0] === '1' ? movieSideBar : ""}
            {selectedMenuKey[0] === '2' ? actorSideBar : ""}
            {selectedMenuKey[0] === '3' ? genreSideBar : ""}
            {selectedMenuKey[0] === '4' ? tagSideBar : ""} 
          </Sider>
          <Layout>
            <Content>
              {selectedMenuKey[0] === '1' ? <MovieViewer ref={movieViewer} searchType="Title"/> : ""}
              {selectedMenuKey[0] === '2' ? <ActorViewer ref={actorViewer} /> : ""}
              {selectedMenuKey[0] === '3' ? <GenreViewer ref={genreViewer} /> : ""}
              {selectedMenuKey[0] === '4' ? <TagViewer ref={tagViewer} /> : ""}
              {selectedMenuKey[0] === '5' ? <DirectorViewer ref={directorViewer} /> : ""}
              {selectedMenuKey[0] === '0' ? <SettingsViewer ref={settings}/> : ""}
            </Content>
          </Layout>
        </Layout>
      </Layout>
    </div>
  );
}

export default App;
